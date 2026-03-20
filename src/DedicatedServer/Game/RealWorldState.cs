using System;
using System.Collections.Generic;
using System.Linq;
using ProcGenGame;

namespace DedicatedServer.Game;

/// <summary>
/// Reads real world state from the loaded game's Grid.
/// Works with both SimDLL-owned memory and pinned managed arrays.
/// </summary>
public class RealWorldState {

    private readonly int width;
    private readonly int height;
    private readonly WorldBuilder world;

    public RealWorldState(int width, int height, WorldBuilder world) {
        this.width = width;
        this.height = height;
        this.world = world;
    }

    public unsafe object GetWorldSnapshot() {
        var numCells = width * height;

        var cells = new object[numCells];
        for (var i = 0; i < numCells; i++) {
            ushort elementIdx = 0;
            float temp = 0f;
            float mass = 0f;

            if (Grid.elementIdx != null) elementIdx = Grid.elementIdx[i];
            if (Grid.temperature != null) temp = Grid.temperature[i];
            if (Grid.mass != null) mass = Grid.mass[i];
            else if (elementIdx < ElementLoader.elements?.Count) {
                mass = ElementLoader.elements[elementIdx].defaultValues.mass;
            }

            cells[i] = new {
                element = (int)elementIdx,
                temperature = Math.Round(temp, 1),
                mass = Math.Round(mass, 1)
            };
        }

        return new {
            width,
            height,
            tick = world.SimTick,
            cells
        };
    }

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

    public object GetEntities() {
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
            foreach (var e in spawnData.otherEntities)
                entities.Add(new { type = "entity", name = e.id, x = e.location_x, y = e.location_y, w = 1, h = 2 });
            foreach (var p in spawnData.pickupables)
                entities.Add(new { type = "pickupable", name = p.id, x = p.location_x, y = p.location_y, w = 1, h = 1 });
            foreach (var o in spawnData.elementalOres)
                entities.Add(new { type = "ore", name = o.id, x = o.location_x, y = o.location_y, w = 1, h = 1 });
        }

        return new {
            tick = world.SimTick,
            entities = entities.ToArray()
        };
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
            duplicantCount = 3, // starter dupes from WorldGen
            buildingCount = world.SpawnData?.buildings?.Count ?? 0,
            entityCount = (world.SpawnData?.otherEntities?.Count ?? 0) +
                          (world.SpawnData?.elementalOres?.Count ?? 0) +
                          (world.SpawnData?.pickupables?.Count ?? 0),
            source = world.SimRunning ? "simdll" : "fallback"
        };
    }
}
