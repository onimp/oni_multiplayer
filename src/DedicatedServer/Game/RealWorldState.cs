using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using ProcGenGame;
using UnityEngine;

namespace DedicatedServer.Game;

/// <summary>
/// Reads real world state from the loaded game's Grid.
/// Works with both SimDLL-owned memory and pinned managed arrays.
/// </summary>
public class RealWorldState {

    private readonly int width;
    private readonly int height;
    private readonly WorldBuilder world;

    // --- World snapshot cache (TTL-based, 200ms) ---
    private byte[]? _worldJsonBytes;
    private System.DateTime _worldCacheTime = System.DateTime.MinValue;
    private static readonly System.TimeSpan WorldCacheTTL = System.TimeSpan.FromMilliseconds(1000);
    private readonly object _worldCacheLock = new();

    // --- Entities cache (static portion — buildings/ores/pickupables never change) ---
    // Live duplicant state is NOT cached here — dupes are recomputed fresh on each call
    // from world.SpawnedMinions so currentChore, smState, navIsMoving, navCell are up-to-date.
    private List<object>? _staticEntities;
    private readonly object _entitiesCacheLock = new();

    // --- State cache (time-based, 200ms TTL) ---
    private byte[]? _stateJsonBytes;
    private long _stateLastMs;
    private const long StateCacheMs = 1000;
    private readonly object _stateCacheLock = new();

    // --- Merged entity size map (built once on first call to GetEntitiesBytes) ---
    // Combines WorldBuilder.PrefabSizeMap (entities/critters) with building sizes from
    // WorldBuilder.BuildingDefCache — buildings always win over stale PrefabSizeMap entries.
    private Dictionary<string, (int w, int h)>? _mergedSizeMap;

    public RealWorldState(int width, int height, WorldBuilder world) {
        this.width = width;
        this.height = height;
        this.world = world;
    }

    /// <summary>
    /// Returns pre-serialized JSON bytes for the world snapshot.
    /// Uses flat primitive arrays instead of per-cell anonymous objects to eliminate
    /// reflection overhead (~100x faster than object[98304] approach).
    /// Cached per SimTick — subsequent requests in the same tick cost ~0ms.
    /// </summary>
    public unsafe byte[] GetWorldSnapshotBytes() {
        var tick = world.SimTick;

        lock (_worldCacheLock) {
            if (_worldJsonBytes != null && (System.DateTime.UtcNow - _worldCacheTime) < WorldCacheTTL) {
                Console.WriteLine($"[WorldState] Cache HIT: world age={(System.DateTime.UtcNow - _worldCacheTime).TotalMilliseconds:F0}ms size={_worldJsonBytes.Length/1024}KB");
                return _worldJsonBytes;
            }
        }

        var sw = Stopwatch.StartNew();
        var numCells = width * height;

        // Flat arrays: vastly faster than object[N] — Newtonsoft uses array serializer (no reflection per element)
        var elements = new int[numCells];
        var temps = new float[numCells];
        var masses = new float[numCells];

        var hasTemp = Grid.temperature != null;
        var hasMass = Grid.mass != null;
        var hasElem = Grid.elementIdx != null;
        var hasElements = ElementLoader.elements != null;

        for (var i = 0; i < numCells; i++) {
            var eidx = hasElem ? Grid.elementIdx![i] : (ushort)0;
            elements[i] = eidx;
            temps[i] = hasTemp ? Grid.temperature![i] : 0f;
            masses[i] = hasMass ? Grid.mass![i] :
                        (hasElements && eidx < ElementLoader.elements!.Count)
                            ? ElementLoader.elements![eidx].defaultValues.mass : 0f;
        }

        var json = JsonConvert.SerializeObject(new { width, height, tick, e = elements, t = temps, m = masses });
        var bytes = Encoding.UTF8.GetBytes(json);
        sw.Stop();
        Console.WriteLine($"[WorldState] Cache MISS: rebuilt world snapshot {bytes.Length / 1024}KB in {sw.ElapsedMilliseconds}ms (tick={tick})");

        lock (_worldCacheLock) {
            _worldCacheTime = System.DateTime.UtcNow;
            _worldJsonBytes = bytes;
        }
        return bytes;
    }

    /// <summary>
    /// Legacy object-returning path kept for compatibility; unused by WebServer (which calls GetWorldSnapshotBytes).
    /// </summary>
    public object GetWorldSnapshot() => new { width, height, tick = world.SimTick, note = "use /api/world" };

    public object GetElements() {
        var elements = new List<object>();
        if (ElementLoader.elements != null) {
            for (var i = 0; i < ElementLoader.elements.Count; i++) {
                var elem = ElementLoader.elements[i];
                elements.Add(new {
                    id = i,
                    name = elem.name ?? $"Element_{i}",
                    state = elem.state.ToString()
                });
            }
        }
        return new { elements };
    }

    // Duplicant prefab IDs (vanilla + Bionic DLC)
    private static readonly HashSet<string> DuplicantPrefabs = new HashSet<string> {
        "Minion", "BionicMinion"
    };

    // Known critter prefab name fragments (case-insensitive)
    private static readonly string[] CritterFragments = {
        "Hatch", "Puft", "Drecko", "Slickster", "Pacu", "Mole", "Morb",
        "Oilfloater", "Squirrel", "Crab", "Shine", "Drecklet", "Puflet",
        "LightBug", "Spider", "Divergent", "Dreep", "Glop", "Flipped"
    };

    private static string ClassifyOtherEntity(string id) {
        if (DuplicantPrefabs.Contains(id)) return "duplicant";
        foreach (var fragment in CritterFragments)
            if (id.IndexOf(fragment, StringComparison.OrdinalIgnoreCase) >= 0) return "critter";
        return "entity";
    }

    /// <summary>
    /// Resolves entity size from the pre-built merged size map.
    /// <para>
    /// The <paramref name="sizeMap"/> is a single unified dictionary that combines:
    ///   • WorldBuilder.BuildingDefCache sizes (all 342 building types, from configTable harvest)
    ///   • WorldBuilder.PrefabSizeMap (entities, critters, geysers — from CaptureEntitySize)
    /// Buildings always win when both sources have an entry (configTable is authoritative).
    /// </para>
    /// Static so it is unit-testable without live GameObjects.
    /// </summary>
    internal static (int w, int h) ResolveEntitySize(
        string prefabId,
        IReadOnlyDictionary<string, (int w, int h)>? sizeMap) {
        if (sizeMap != null && sizeMap.TryGetValue(prefabId, out var sz) && sz.w > 0 && sz.h > 0)
            return sz;
        return (1, 1);
    }

    /// <summary>
    /// Builds the merged size map (computed once; entities are static after world load).
    /// Starts from PrefabSizeMap (entities/critters), then overlays building sizes from
    /// BuildingDefCache — buildings always win over any stale PrefabSizeMap entry.
    /// </summary>
    private Dictionary<string, (int w, int h)> BuildMergedSizeMap() {
        var map = world.PrefabSizeMap.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        foreach (var kvp in WorldBuilder.BuildingDefCache) {
            var size = WorldBuilder.ReadBuildingDefSize(kvp.Value);
            if (size.HasValue) map[kvp.Key] = size.Value;
        }
        Console.WriteLine($"[SizeMap] Built merged size map: {map.Count} entries " +
            $"(prefabSizeMap={world.PrefabSizeMap.Count} buildingDefCache={WorldBuilder.BuildingDefCache.Count})");
        return map;
    }

    public object GetEntities() {
        return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(GetEntitiesBytes()))!;
    }

    // ─── Minion live-state DTO ──────────────────────────────────────────────────

    /// <summary>
    /// Builds a duplicant entity DTO from live GO state.
    /// Called on every <see cref="GetEntitiesBytes"/> request (not cached) so the
    /// client always sees current chore, SM state, and nav position.
    /// </summary>
    private object BuildLiveMinionDto(GameObject go) {
        _mergedSizeMap ??= BuildMergedSizeMap();
        var (w, h) = ResolveEntitySize("Minion", _mergedSizeMap);

        var pos = go.transform.GetPosition();
        // Use floor (same as Grid.PosToCell) — objects sit at cell+0.5 center, so
        // RoundToInt(5.5)=6 would be off by one; (int)5.5=5 gives the correct cell column.
        var x   = (int)pos.x;
        var y   = (int)pos.y;

        // Current chore: ChoreDriver.GetCurrentChore().choreType.Name (null = no chore)
        string? currentChore = null;
        var choreDriver = go.GetComponent<ChoreDriver>();
        if (choreDriver?.GetSMI() != null)
            currentChore = choreDriver.GetCurrentChore()?.choreType?.Name;

        // SM state: first non-null SM in the controller — generic, works for any entity type.
        // smc.stateMachines is ordered by initialization; most relevant SMs come first.
        // Returns descriptive path-style names like "root.alive.notasleep.idle" or "nochore".
        var smc     = go.GetComponent<StateMachineController>();
        var smState = smc?.stateMachines?.FirstOrDefault(s => s != null)?.GetCurrentState()?.name ?? "none";

        // Navigator: IsMoving + current cell
        var nav         = go.GetComponent<Navigator>();
        var navIsMoving = nav?.IsMoving() ?? false;
        var navCell     = nav != null ? Grid.PosToCell(go) : -1;

        var name = go.GetComponent<MinionIdentity>()?.nameStringKey ?? go.name;
        return BuildMinionDto(name, x, y, w, h, currentChore, smState, navIsMoving, navCell);
    }

    /// <summary>
    /// Builds a critter entity DTO from live GO state. Critters are NOT static-cached:
    /// they move, change SM state, and die — so they must be recomputed each request.
    /// smState uses the same smc.stateMachines.FirstOrDefault() pattern as dupes.
    /// </summary>
    private object BuildLiveCreatureDto(GameObject go) {
        _mergedSizeMap ??= BuildMergedSizeMap();
        var prefabId = go.GetComponent<KPrefabID>()?.PrefabTag.Name ?? go.name;
        var (w, h)   = ResolveEntitySize(prefabId, _mergedSizeMap);
        var pos      = go.transform.GetPosition();
        var x        = (int)pos.x;
        var y        = (int)pos.y;
        var smc      = go.GetComponent<StateMachineController>();
        var smState  = smc?.stateMachines?.FirstOrDefault(s => s != null)?.GetCurrentState()?.name ?? "none";
        return new { type = "critter", name = prefabId, x, y, w, h, smState };
    }

    /// <summary>
    /// Constructs the duplicant entity DTO with live chore/SM/nav fields.
    /// <para>
    /// Exposed as <c>internal static</c> so unit tests can verify the JSON shape
    /// without requiring a running game instance.
    /// </para>
    /// Fields:
    ///   type         — always "duplicant"
    ///   name         — duplicant's personal name (MinionIdentity.nameStringKey, e.g. "Aaron")
    ///   x, y         — world position (rounded to int)
    ///   w, h         — bounding box in cells (typically 1×2 for a dupe)
    ///   currentChore — ChoreType.Name of the active chore, or null when idle/no chore
    ///   smState      — first SM state in StateMachineController (path-style, e.g. "root.alive.idle")
    ///   navIsMoving  — true when Navigator is executing a path
    ///   navCell      — grid cell index at current position (Grid.PosToCell)
    /// </summary>
    internal static object BuildMinionDto(
        string  name,
        int     x,
        int     y,
        int     w,
        int     h,
        string? currentChore,
        string? smState,
        bool    navIsMoving,
        int     navCell)
    {
        return new {
            type         = "duplicant",
            name,
            x, y, w, h,
            currentChore,
            smState,
            navIsMoving,
            navCell
        };
    }

    // ─── Entities serialization ────────────────────────────────────────────────

    /// <summary>
    /// Returns pre-serialized JSON bytes for the entity list.
    ///
    /// Static entities (buildings, ores, pickupables, non-dupe critters) are cached
    /// permanently — SpawnData never changes after world load.
    ///
    /// Duplicant entries are NOT cached: they are recomputed on every call from
    /// <c>world.SpawnedMinions</c> so <c>currentChore</c>, <c>smState</c>,
    /// <c>navIsMoving</c>, and <c>navCell</c> always reflect current game state.
    /// </summary>
    public byte[] GetEntitiesBytes() {
        // [EntityCollect] diagnostic: runs every call so the pipeline state is always visible.
        {
            var diagSpawnData = world.SpawnData;
            var diagTracked   = world.TrackedBuildings;
            Console.WriteLine(
                $"[EntityCollect] spawnData={(diagSpawnData != null ? "OK" : "NULL")} " +
                $"buildings={diagSpawnData?.buildings?.Count ?? -1} " +
                $"trackedBuildings={diagTracked?.Count ?? -1} " +
                $"otherEntities={diagSpawnData?.otherEntities?.Count ?? -1} " +
                $"elementalOres={diagSpawnData?.elementalOres?.Count ?? -1} " +
                $"pickupables={diagSpawnData?.pickupables?.Count ?? -1} " +
                $"directlySpawned={world.DirectlySpawnedEntities?.Count ?? -1} " +
                $"spawnedMinions={world.SpawnedMinions?.Count ?? -1}");
            if (diagTracked != null && diagTracked.Count > 0) {
                var preview = string.Join(", ", diagTracked.Take(10).Select(b => b.id));
                var hasHq   = diagTracked.Any(b => b.id == "Headquarters");
                Console.WriteLine($"[EntityCollect] trackedBuildings[0..9]=[{preview}]  hasHeadquarters={hasHq}");
            }
        }

        // ── Step 1: static entities (permanent cache) ──────────────────────────
        lock (_entitiesCacheLock) {
            if (_staticEntities == null) {
                _staticEntities = BuildStaticEntities();
            }
        }

        // ── Step 2: live duplicant DTOs (always fresh — never cached) ──────────
        var liveMinions = new List<object>();
        foreach (var go in world.SpawnedMinions) {
            if (go == null) continue;
            liveMinions.Add(BuildLiveMinionDto(go));
        }

        // ── Step 2b: live critter DTOs (always fresh — critters move and change state) ──
        // Critters are NOT in _staticEntities (excluded in BuildStaticEntities).
        // We iterate Components.Brains for all CreatureBrain instances — this covers
        // every critter that went through TriggerLifecycle during SpawnEntities().
        var liveCreatures = new List<object>();
        foreach (var brain in Components.Brains.Items) {
            if (brain == null || brain is MinionBrain) continue;
            if (brain.gameObject == null) continue;
            try { liveCreatures.Add(BuildLiveCreatureDto(brain.gameObject)); }
            catch { /* skip individual critter DTO build failures */ }
        }

        // ── Step 3: combine and serialize ──────────────────────────────────────
        var allEntities = new List<object>(_staticEntities!.Count + liveMinions.Count + liveCreatures.Count);
        allEntities.AddRange(_staticEntities!);
        allEntities.AddRange(liveMinions);
        allEntities.AddRange(liveCreatures);

        return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {
            tick     = world.SimTick,
            entities = allEntities.ToArray()
        }));
    }

    /// <summary>
    /// Builds the list of static (non-dupe, non-critter) entities from SpawnData and TrackedBuildings.
    /// Duplicant prefabs (Minion, BionicMinion) are excluded — serialized live via BuildLiveMinionDto.
    /// Critters are excluded — serialized live via BuildLiveCreatureDto (they move and change state).
    /// Buildings, ores, pickupables, geysers are static — cached permanently after first call.
    /// </summary>
    private List<object> BuildStaticEntities() {
        var sw       = Stopwatch.StartNew();
        var entities = new List<object>();
        var spawnData = world.SpawnData;

        // Build the merged size map once: buildings (from BuildingDefCache) always win.
        _mergedSizeMap ??= BuildMergedSizeMap();
        var sizeMap = _mergedSizeMap;

        // Buildings (TrackedBuildings has world-offsets already applied).
        foreach (var b in world.TrackedBuildings) {
            var (w, h) = ResolveEntitySize(b.id, sizeMap);
            entities.Add(new { type = "building", name = b.id, x = b.x, y = b.y, w, h });
        }

        if (spawnData != null) {
            foreach (var e in spawnData.otherEntities) {
                if (DuplicantPrefabs.Contains(e.id)) continue;   // handled via SpawnedMinions
                if (ClassifyOtherEntity(e.id) == "critter") continue;  // handled live via Components.Brains
                var (ew, eh) = ResolveEntitySize(e.id, sizeMap);
                entities.Add(new { type = ClassifyOtherEntity(e.id), name = e.id, x = e.location_x, y = e.location_y, w = ew, h = eh });
            }
            foreach (var p in spawnData.pickupables) {
                var (pw, ph) = ResolveEntitySize(p.id, sizeMap);
                entities.Add(new { type = "pickupable", name = p.id, x = p.location_x, y = p.location_y, w = pw, h = ph });
            }
            foreach (var o in spawnData.elementalOres) {
                var (ow, oh) = ResolveEntitySize(o.id, sizeMap);
                entities.Add(new { type = "ore", name = o.id, x = o.location_x, y = o.location_y, w = ow, h = oh });
            }
        }

        // Directly-spawned non-dupe, non-critter entities (geysers, etc.).
        foreach (var (id, x, y) in world.DirectlySpawnedEntities) {
            if (DuplicantPrefabs.Contains(id)) continue;
            if (ClassifyOtherEntity(id) == "critter") continue;  // handled live via Components.Brains
            var (ew, eh) = ResolveEntitySize(id, sizeMap);
            entities.Add(new { type = ClassifyOtherEntity(id), name = id, x, y, w = ew, h = eh });
        }

        sw.Stop();
        Console.WriteLine($"[WorldState] Built static entities list: {entities.Count} entities in {sw.ElapsedMilliseconds}ms");
        return entities;
    }

    /// <summary>
    /// Returns pre-serialized JSON bytes for game state. Cached with 200ms TTL.
    /// </summary>
    public byte[] GetGameStateBytes() {
        var nowMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        lock (_stateCacheLock) {
            if (_stateJsonBytes != null && nowMs - _stateLastMs < StateCacheMs) {
                Console.WriteLine($"[WorldState] Cache HIT: state age={nowMs - _stateLastMs}ms");
                return _stateJsonBytes;
            }
        }

        var gameClock = GameClock.Instance;
        var cycle = gameClock != null ? gameClock.GetCycle() + 1 : 1;
        var obj = new {
            tick = world.SimTick,
            cycle,
            speed = 1,
            paused = !world.SimRunning,
            worldWidth = width,
            worldHeight = height,
            duplicantCount = 3,
            buildingCount = world.SpawnData?.buildings?.Count ?? 0,
            entityCount = (world.SpawnData?.otherEntities?.Count ?? 0) +
                          (world.SpawnData?.elementalOres?.Count ?? 0) +
                          (world.SpawnData?.pickupables?.Count ?? 0),
            source = world.SimRunning ? "simdll" : "fallback",
            serverUps = world.TickLoop?.Ups ?? 0
        };
        var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        Console.WriteLine($"[WorldState] Cache MISS: rebuilt state {bytes.Length}B");
        lock (_stateCacheLock) {
            _stateLastMs = nowMs;
            _stateJsonBytes = bytes;
        }
        return bytes;
    }

    public object GetGameState() {
        var gameClock = GameClock.Instance;
        var cycle = gameClock != null ? gameClock.GetCycle() + 1 : 1;
        return new {
            tick = world.SimTick,
            cycle,
            speed = 1,
            paused = !world.SimRunning,
            worldWidth = width,
            worldHeight = height,
            duplicantCount = 3,
            buildingCount = world.SpawnData?.buildings?.Count ?? 0,
            entityCount = (world.SpawnData?.otherEntities?.Count ?? 0) +
                          (world.SpawnData?.elementalOres?.Count ?? 0) +
                          (world.SpawnData?.pickupables?.Count ?? 0),
            source = world.SimRunning ? "simdll" : "fallback"
        };
    }
}
