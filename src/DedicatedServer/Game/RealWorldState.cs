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

        // Build the merged size map once: buildings (from BuildingDefCache) always win over
        // any stale PrefabSizeMap entry. All branches below use the same single source.
        _mergedSizeMap ??= BuildMergedSizeMap();
        var sizeMap = _mergedSizeMap;

        // Buildings branch: use TrackedBuildings (populated during SpawnEntities with offsets
        // already applied) so the buildings list is independent of SpawnData validity at query time.
        foreach (var b in world.TrackedBuildings) {
            var (w, h) = ResolveEntitySize(b.id, sizeMap);
            Console.WriteLine($"[EntityPath] id={b.id} → branch=buildings size={w}×{h}");
            entities.Add(new { type = "building", name = b.id, x = b.x, y = b.y, w, h });
        }

        if (spawnData != null) {
            foreach (var e in spawnData.otherEntities) {
                var (ew, eh) = ResolveEntitySize(e.id, sizeMap);
                Console.WriteLine($"[EntityPath] id={e.id} → branch=otherEntities size={ew}×{eh}");
                entities.Add(new { type = ClassifyOtherEntity(e.id), name = e.id, x = e.location_x, y = e.location_y, w = ew, h = eh });
            }
            foreach (var p in spawnData.pickupables) {
                var (pw, ph) = ResolveEntitySize(p.id, sizeMap);
                Console.WriteLine($"[EntityPath] id={p.id} → branch=pickupables size={pw}×{ph}");
                entities.Add(new { type = "pickupable", name = p.id, x = p.location_x, y = p.location_y, w = pw, h = ph });
            }
            foreach (var o in spawnData.elementalOres) {
                var (ow, oh) = ResolveEntitySize(o.id, sizeMap);
                Console.WriteLine($"[EntityPath] id={o.id} → branch=elementalOres size={ow}×{oh}");
                entities.Add(new { type = "ore", name = o.id, x = o.location_x, y = o.location_y, w = ow, h = oh });
            }
        }

        // Include entities spawned directly (e.g. starter minions via SpawnStarterMinions).
        foreach (var (id, x, y) in world.DirectlySpawnedEntities) {
            var (ew, eh) = ResolveEntitySize(id, sizeMap);
            Console.WriteLine($"[EntityPath] id={id} → branch=DirectlySpawnedEntities size={ew}×{eh}");
            entities.Add(new { type = ClassifyOtherEntity(id), name = id, x, y, w = ew, h = eh });
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
