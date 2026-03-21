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

    // --- World snapshot cache (invalidated per SimTick) ---
    private byte[]? _worldJsonBytes;
    private int _worldCacheTick = -1;
    private readonly object _worldCacheLock = new();

    // --- Entities cache (static per world — SpawnData never changes) ---
    private byte[]? _entitiesBytes;
    private readonly object _entitiesCacheLock = new();

    // --- State cache (time-based, 200ms TTL) ---
    private byte[]? _stateJsonBytes;
    private long _stateLastMs;
    private const long StateCacheMs = 200;
    private readonly object _stateCacheLock = new();

    // --- Entity size cache (static — prefab sizes don't change) ---
    private static readonly Dictionary<string, (int w, int h)> _entitySizeCache = new();

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
            if (_worldJsonBytes != null && _worldCacheTick == tick) {
                Console.WriteLine($"[WorldState] Cache HIT: world tick={tick} size={_worldJsonBytes.Length/1024}KB");
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
            _worldCacheTick = tick;
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
    /// Returns entity size in cells by inspecting OccupyArea component on the registered prefab.
    /// Falls back to KBoxCollider2D size, then (1,1). Results are cached.
    /// </summary>
    private static (int w, int h) GetEntitySize(string id) {
        if (_entitySizeCache.TryGetValue(id, out var cached)) return cached;

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
                    // Fallback: KBoxCollider2D size
                    var col = prefab.GetComponent<KBoxCollider2D>();
                    if (col != null) {
                        var s = col.size;
                        size = (Math.Max(1, (int)Math.Round(s.x)), Math.Max(1, (int)Math.Round(s.y)));
                    }
                }
            }
        } catch { /* use default */ }

        _entitySizeCache[id] = size;
        return size;
    }

    public object GetEntities() {
        return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(GetEntitiesBytes()))!;
    }

    /// <summary>
    /// Returns pre-serialized JSON bytes for the entity list.
    /// Cached permanently — SpawnData never changes after world load.
    /// </summary>
    public byte[] GetEntitiesBytes() {
        lock (_entitiesCacheLock) {
            if (_entitiesBytes != null) return _entitiesBytes;
        }

        var sw = Stopwatch.StartNew();
        var entities = new List<object>();
        var spawnData = world.SpawnData;

        if (spawnData != null) {
            foreach (var b in spawnData.buildings) {
                int w = 1, h = 1;
                try {
                    var def = world.GetBuildingDef(b.id);
                    if (def != null) { w = def.WidthInCells; h = def.HeightInCells; }
                } catch { /* BuildingDef not registered */ }
                entities.Add(new {
                    type = "building", name = b.id, x = b.location_x, y = b.location_y, w, h
                });
            }
            foreach (var e in spawnData.otherEntities) {
                var entityType = ClassifyOtherEntity(e.id);
                var (ew, eh) = GetEntitySize(e.id);
                entities.Add(new { type = entityType, name = e.id, x = e.location_x, y = e.location_y, w = ew, h = eh });
            }
            foreach (var p in spawnData.pickupables) {
                var (pw, ph) = GetEntitySize(p.id);
                entities.Add(new { type = "pickupable", name = p.id, x = p.location_x, y = p.location_y, w = pw, h = ph });
            }
            foreach (var o in spawnData.elementalOres)
                entities.Add(new { type = "ore", name = o.id, x = o.location_x, y = o.location_y, w = 1, h = 1 });
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
