using System;
using System.Collections.Generic;

namespace DedicatedServer.Game;

/// <summary>
/// Reads real world state from the loaded game's Grid.
/// Replaces MockWorldState when game is loaded.
/// </summary>
public class RealWorldState {

    private readonly int width;
    private readonly int height;
    private int tick;

    public RealWorldState(int width, int height) {
        this.width = width;
        this.height = height;
    }

    public unsafe object GetWorldSnapshot() {
        tick++;
        var numCells = width * height;

        var cells = new object[numCells];
        for (var i = 0; i < numCells; i++) {
            ushort elementIdx = 0;
            float temp = 0f;
            float mass = 0f;

            if (Grid.elementIdx != null) elementIdx = Grid.elementIdx[i];
            if (Grid.temperature != null) temp = Grid.temperature[i];

            // Get default mass from element definition
            if (elementIdx < ElementLoader.elements?.Count) {
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
            tick,
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
        return new {
            tick,
            entities = entities.ToArray()
        };
    }

    public object GetGameState() {
        var gameClock = GameClock.Instance;
        var cycle = gameClock != null ? gameClock.GetCycle() + 1 : 1;

        return new {
            tick,
            cycle,
            speed = 1,
            paused = false,
            worldWidth = width,
            worldHeight = height,
            duplicantCount = 0,
            buildingCount = 0,
            source = "game"
        };
    }
}
