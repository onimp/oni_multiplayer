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

    // --- Entities cache (static per world — SpawnData never changes) ---
    private byte[]? _entitiesBytes;
    private readonly object _entitiesCacheLock = new();

    // --- State cache (time-based, 200ms TTL) ---
    private byte[]? _stateJsonBytes;
    private long _stateLastMs;
    private const long StateCacheMs = 1000;
    private readonly object _stateCacheLock = new();

    // --- Entity size cache (per-instance — populated from WorldBuilder.PrefabSizeMap on first use) ---
    private readonly Dictionary<string, (int w, int h)> _entitySizeCache = new();

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
    /// Returns entity size in cells.
    /// Priority order:
    ///   1. _entitySizeCache — per-instance result cache
    ///   2. WorldBuilder.BuildingDefCache — static, from configTable harvest (authoritative for
    ///      buildings that failed Add2DComponents: HQ, Telepad, GeneShuffler). Checked BEFORE
    ///      PrefabSizeMap because CaptureEntitySize can overwrite PrefabSizeMap with stale (1,1)
    ///      — that's the "third silent path" that returns (1,1) with no log for HQ.
    ///   3. WorldBuilder.PrefabSizeMap — from SpawnEntities CaptureEntitySize (may be stale 1,1)
    ///   4. Assets.GetPrefab OccupyArea → KBoxCollider2D → (1,1)
    /// </summary>
    private (int w, int h) GetEntitySize(string id) {
        if (_entitySizeCache.TryGetValue(id, out var cached)) return cached;

        // Primary: static BuildingDefCache — from configTable harvest, never overwritten by
        // spawn-time logic. For buildings whose Add2DComponents crashed, this is the ONLY
        // reliable source. Must be checked before PrefabSizeMap which can hold stale (1,1).
        try {
            if (WorldBuilder.BuildingDefCache.TryGetValue(id, out var buildingDef)) {
                var defSize = WorldBuilder.ReadBuildingDefSize(buildingDef);
                if (defSize.HasValue) {
                    Console.WriteLine($"[EntitySize] {id} → {defSize.Value.w}×{defSize.Value.h} (BuildingDefCache)");
                    _entitySizeCache[id] = defSize.Value;
                    return defSize.Value;
                }
            }
        } catch (Exception ex) {
            Console.WriteLine($"[EntitySize] {id} BuildingDefCache threw: {ex.GetBaseException().Message}");
        }

        // Secondary: PrefabSizeMap — from SpawnEntities CaptureEntitySize (critters, dupes, etc.)
        // NOTE: this was formerly primary. For buildings it can hold stale (1,1) if CaptureEntitySize
        // ran before configTable harvest or failed to read Building.Def in headless.
        if (world.PrefabSizeMap.TryGetValue(id, out var liveSize)) {
            _entitySizeCache[id] = liveSize;
            return liveSize;
        }

        // Fallback: inspect the registered prefab in Assets
        (int w, int h) size = (1, 1);
        try {
            var prefab = Assets.GetPrefab(new Tag(id));
            if (prefab != null) {
                var occupyArea = prefab.GetComponent<OccupyArea>();
                if (occupyArea?._UnrotatedOccupiedCellsOffsets?.Length > 0) {
                    var offsets = occupyArea._UnrotatedOccupiedCellsOffsets;
                    int minX = 0, maxX = 0, minY = 0, maxY = 0;
                    foreach (var o in offsets) {
                        if (o.x < minX) minX = o.x;
                        if (o.x > maxX) maxX = o.x;
                        if (o.y < minY) minY = o.y;
                        if (o.y > maxY) maxY = o.y;
                    }
                    size = (maxX - minX + 1, maxY - minY + 1);
                } else {
                    // Fallback: KBoxCollider2D size (set in ConfigPlacedEntity alongside OccupyArea)
                    var col = prefab.GetComponent<KBoxCollider2D>();
                    if (col != null) {
                        var s = col.size;
                        size = (Math.Max(1, (int)Math.Round(s.x)), Math.Max(1, (int)Math.Round(s.y)));
                    }
                }
                Console.WriteLine($"[EntitySize] {id} → {size.w}×{size.h} (Assets fallback, occupyArea={(prefab.GetComponent<OccupyArea>() != null ? "found" : "null")})");
            } else {
                Console.WriteLine($"[EntitySize] {id} → prefab not found in Assets, defaulting to 1×1");
            }
        } catch (Exception ex) {
            Console.WriteLine($"[EntitySize] {id} → exception: {ex.GetBaseException().Message}, defaulting to 1×1");
        }

        _entitySizeCache[id] = size;
        return size;
    }

    /// <summary>
    /// Resolves building cell dimensions from three sources in priority order.
    /// 1. <paramref name="buildingDefCache"/> — static _buildingDefCache populated during
    ///    RegisterBuildingDefs() configTable harvest. Contains ALL 342 defs including buildings
    ///    that failed full registration (HQ, Telepad) — dimensions read directly via
    ///    WorldBuilder.ReadBuildingDefSize, no game API call needed.
    /// 2. <paramref name="getBuildingDef"/> — Assets.GetBuildingDef wrapped — only works for
    ///    buildings that completed registration successfully.
    /// 3. <paramref name="sizeMap"/> — PrefabSizeMap populated during SpawnEntities().
    ///    May have stale (1,1) entries if CaptureEntitySize fell through in headless.
    /// Static so it can be unit-tested without live GameObjects or game APIs.
    /// </summary>
    internal static (int w, int h) ResolveBuildingSize(
        string id,
        IReadOnlyDictionary<string, (int w, int h)> sizeMap,
        IReadOnlyDictionary<string, BuildingDef> buildingDefCache,
        Func<string, BuildingDef> getBuildingDef) {
        Console.WriteLine($"[Size] Resolving id={id} → checking buildingDefCache(count={buildingDefCache?.Count ?? -1})...");
        // Primary: direct lookup in _buildingDefCache — populated from configTable harvest even
        // for buildings whose Add2DComponents crashed (HQ, Telepad, GeneShuffler).
        // Wrapped in try-catch: BuildingDef property accessors (e.g. WidthInCells) can throw in
        // headless mode after partial initialization — must not bypass the sizeMap fallback.
        try {
            if (buildingDefCache != null && buildingDefCache.TryGetValue(id, out var cachedDef)) {
                var cacheSize = WorldBuilder.ReadBuildingDefSize(cachedDef);
                Console.WriteLine($"[Size]   buildingDefCache hit: def={cachedDef?.PrefabID ?? "null"} cacheSize={cacheSize?.ToString() ?? "null"}");
                if (cacheSize.HasValue) return cacheSize.Value;
            } else {
                Console.WriteLine($"[Size]   buildingDefCache miss for id={id}");
            }
        } catch (Exception ex) {
            Console.WriteLine($"[Size]   buildingDefCache threw: {ex.GetBaseException().Message} — falling through to getBuildingDef");
        }
        // Secondary: Assets.GetBuildingDef (works for fully-registered buildings).
        try {
            var def = getBuildingDef?.Invoke(id);
            if (def != null && def.WidthInCells > 0 && def.HeightInCells > 0) {
                Console.WriteLine($"[Size]   getBuildingDef hit: {def.WidthInCells}x{def.HeightInCells}");
                return (def.WidthInCells, def.HeightInCells);
            }
        } catch {
            // Game API unavailable (headless or test context) — fall through to sizeMap.
        }
        // Fallback: instance PrefabSizeMap (populated from configTable harvest — authoritative even
        // for buildings that failed Add2DComponents, as long as CreateBuildingDef succeeded).
        if (sizeMap != null && sizeMap.TryGetValue(id, out var sz) && sz.w > 0 && sz.h > 0) {
            Console.WriteLine($"[Size]   sizeMap hit: {sz.w}x{sz.h}");
            return sz;
        }
        Console.WriteLine($"[Size]   all sources failed → 1x1");
        return (1, 1);
    }

    public object GetEntities() {
        return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(GetEntitiesBytes()))!;
    }

    /// <summary>
    /// Returns pre-serialized JSON bytes for the entity list.
    /// Cached permanently — SpawnData never changes after world load.
    /// </summary>
    public byte[] GetEntitiesBytes() {
        // [EntityCollect] diagnostic: runs every call (including cache hits) so the pipeline
        // state is always visible in server logs without waiting for a cache-cold run.
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
                $"directlySpawned={world.DirectlySpawnedEntities?.Count ?? -1}");
            if (diagTracked != null && diagTracked.Count > 0) {
                var preview = string.Join(", ", diagTracked.Take(10).Select(b => b.id));
                var hasHq   = diagTracked.Any(b => b.id == "Headquarters");
                Console.WriteLine($"[EntityCollect] trackedBuildings[0..9]=[{preview}]  hasHeadquarters={hasHq}");
            }
        }

        lock (_entitiesCacheLock) {
            if (_entitiesBytes != null) return _entitiesBytes;
        }

        var sw = Stopwatch.StartNew();
        var entities = new List<object>();
        var spawnData = world.SpawnData;

        // Buildings branch: use TrackedBuildings (populated during SpawnEntities with offsets
        // already applied) instead of spawnData.buildings.  This guarantees buildings appear
        // in the entity list regardless of SpawnData reference validity at query time.
        foreach (var b in world.TrackedBuildings) {
            Console.WriteLine($"[EntityPath] id={b.id} → branch=buildings");
            var (w, h) = ResolveBuildingSize(b.id, world.PrefabSizeMap, WorldBuilder.BuildingDefCache, world.GetBuildingDef);
            entities.Add(new {
                type = "building", name = b.id, x = b.x, y = b.y, w, h
            });
        }

        if (spawnData != null) {
            foreach (var e in spawnData.otherEntities) {
                Console.WriteLine($"[EntityPath] id={e.id} → branch=otherEntities");
                var entityType = ClassifyOtherEntity(e.id);
                var (ew, eh) = GetEntitySize(e.id);
                entities.Add(new { type = entityType, name = e.id, x = e.location_x, y = e.location_y, w = ew, h = eh });
            }
            foreach (var p in spawnData.pickupables) {
                Console.WriteLine($"[EntityPath] id={p.id} → branch=pickupables");
                var (pw, ph) = GetEntitySize(p.id);
                entities.Add(new { type = "pickupable", name = p.id, x = p.location_x, y = p.location_y, w = pw, h = ph });
            }
            foreach (var o in spawnData.elementalOres) {
                Console.WriteLine($"[EntityPath] id={o.id} → branch=elementalOres");
                // Fix: was hardcoded w=1,h=1 — use GetEntitySize so BuildingDefCache
                // is consulted first. If HQ or any other building ends up here (e.g. via
                // the world-gen template serialisation), its real size is returned.
                // Actual ores (Algae, Dirt, etc.) are not in BuildingDefCache and remain 1×1.
                var (ow, oh) = GetEntitySize(o.id);
                entities.Add(new { type = "ore", name = o.id, x = o.location_x, y = o.location_y, w = ow, h = oh });
            }
        }

        // Include entities spawned directly (e.g. starter minions via SpawnStarterMinions).
        // These bypass spawnData.otherEntities so they must be added here explicitly.
        foreach (var (id, x, y) in world.DirectlySpawnedEntities) {
            Console.WriteLine($"[EntityPath] id={id} → branch=DirectlySpawnedEntities");
            var entityType = ClassifyOtherEntity(id);
            var (ew, eh) = GetEntitySize(id);
            entities.Add(new { type = entityType, name = id, x, y, w = ew, h = eh });
        }

        var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {
            tick = world.SimTick,
            entities = entities.ToArray()
        }));
        sw.Stop();
        Console.WriteLine($"[WorldState] Built entities list: {entities.Count} entities in {sw.ElapsedMilliseconds}ms");

        lock (_entitiesCacheLock) {
            _entitiesBytes = bytes;
        }
        return bytes;
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
            source = world.SimRunning ? "simdll" : "fallback"
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
