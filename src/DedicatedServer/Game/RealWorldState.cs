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
    private readonly GameLoader loader;

    public RealWorldState(int width, int height, GameLoader loader) {
        this.width = width;
        this.height = height;
        this.loader = loader;
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
            tick = loader.SimTick,
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
        var spawnData = loader.SpawnData;

        if (spawnData != null) {
            foreach (var b in spawnData.buildings)
                entities.Add(new { type = "building", name = b.id, x = b.location_x, y = b.location_y });
            foreach (var e in spawnData.otherEntities)
                entities.Add(new { type = "entity", name = e.id, x = e.location_x, y = e.location_y });
            foreach (var p in spawnData.pickupables)
                entities.Add(new { type = "pickupable", name = p.id, x = p.location_x, y = p.location_y });
            foreach (var o in spawnData.elementalOres)
                entities.Add(new { type = "ore", name = o.id, x = o.location_x, y = o.location_y });
        }

        return new {
            tick = loader.SimTick,
            entities = entities.ToArray()
        };
    }

    public object GetGameState() {
        var gameClock = GameClock.Instance;
        var cycle = gameClock != null ? gameClock.GetCycle() + 1 : 1;

        return new {
            tick = loader.SimTick,
            cycle,
            speed = 1,
            paused = !loader.SimRunning,
            worldWidth = width,
            worldHeight = height,
            duplicantCount = loader.SpawnData?.otherEntities?.Count ?? 0,
            buildingCount = loader.SpawnData?.buildings?.Count ?? 0,
            source = loader.SimRunning ? "simdll" : "fallback"
        };
    }
}
