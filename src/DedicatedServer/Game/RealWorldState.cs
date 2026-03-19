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

            if (Grid.elementIdx != null) elementIdx = Grid.elementIdx[i];
            if (Grid.temperature != null) temp = Grid.temperature[i];

            cells[i] = new {
                element = (int)elementIdx,
                temperature = Math.Round(temp, 1),
                mass = 0.0
            };
        }

        return new {
            width,
            height,
            tick,
            cells
        };
    }

    public object GetEntities() {
        var entities = new List<object>();

        if (global::Game.Instance != null) {
            // Real entity enumeration will come in Phase 3
        }

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
