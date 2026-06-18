using System;
using System.Collections.Generic;
using System.Linq;

namespace DedicatedServer.Web;

/// <summary>
/// Generates mock world data that mimics ONI's Grid structure.
/// Will be replaced with real game data in Phase 2.
/// </summary>
public class MockWorldState {

    private const int Width = 64;
    private const int Height = 64;
    private readonly Random rng = new(42);
    private int tick;

    // Element types matching ONI's element system
    public enum Element {
        Vacuum = 0,
        Oxygen = 1,
        CarbonDioxide = 2,
        Hydrogen = 3,
        Water = 4,
        DirtyWater = 5,
        Granite = 6,
        SandStone = 7,
        Algae = 8,
        Copper = 9,
        Ice = 10
    }

    private readonly int[] grid;           // element per cell
    private readonly float[] temperature;  // kelvin per cell
    private readonly float[] mass;         // kg per cell
    private readonly List<EntityData> entities;

    public MockWorldState() {
        grid = new int[Width * Height];
        temperature = new float[Width * Height];
        mass = new float[Width * Height];
        entities = new List<EntityData>();

        GenerateWorld();
        SpawnEntities();
    }

    private void GenerateWorld() {
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                var idx = y * Width + x;

                if (y < 5) {
                    // Bottom: solid rock
                    grid[idx] = (int)Element.Granite;
                    temperature[idx] = 300f + rng.Next(-10, 10);
                    mass[idx] = 1000f + rng.Next(0, 500);
                } else if (y < 15) {
                    // Lower area: mix of rock and resources
                    var r = rng.NextDouble();
                    if (r < 0.5) {
                        grid[idx] = (int)Element.SandStone;
                        mass[idx] = 800f + rng.Next(0, 400);
                    } else if (r < 0.7) {
                        grid[idx] = (int)Element.Copper;
                        mass[idx] = 500f + rng.Next(0, 300);
                    } else if (r < 0.85) {
                        grid[idx] = (int)Element.Algae;
                        mass[idx] = 200f + rng.Next(0, 200);
                    } else {
                        grid[idx] = (int)Element.Oxygen;
                        mass[idx] = 1.5f + (float)(rng.NextDouble() * 2);
                    }
                    temperature[idx] = 290f + rng.Next(-5, 15);
                } else if (y < 50) {
                    // Middle: habitable zone — mostly gas with some structures
                    var r = rng.NextDouble();
                    if (y > 20 && y < 25 && (x < 10 || x > 54)) {
                        // Side walls
                        grid[idx] = (int)Element.SandStone;
                        mass[idx] = 1000f;
                    } else if (r < 0.65) {
                        grid[idx] = (int)Element.Oxygen;
                        mass[idx] = 1.8f + (float)(rng.NextDouble() * 1.5);
                    } else if (r < 0.85) {
                        grid[idx] = (int)Element.CarbonDioxide;
                        mass[idx] = 0.5f + (float)(rng.NextDouble() * 1);
                    } else if (r < 0.9) {
                        grid[idx] = (int)Element.Hydrogen;
                        mass[idx] = 0.1f + (float)(rng.NextDouble() * 0.3);
                    } else {
                        grid[idx] = (int)Element.Vacuum;
                        mass[idx] = 0f;
                    }
                    temperature[idx] = 293f + rng.Next(-3, 8);
                } else if (y < 55) {
                    // Upper: cold zone with ice
                    var r = rng.NextDouble();
                    if (r < 0.4) {
                        grid[idx] = (int)Element.Ice;
                        mass[idx] = 500f + rng.Next(0, 500);
                    } else if (r < 0.7) {
                        grid[idx] = (int)Element.Oxygen;
                        mass[idx] = 1.2f;
                    } else {
                        grid[idx] = (int)Element.Granite;
                        mass[idx] = 1200f;
                    }
                    temperature[idx] = 260f + rng.Next(-10, 5);
                } else {
                    // Top: mostly vacuum / thin atmosphere
                    grid[idx] = rng.NextDouble() < 0.3 ? (int)Element.Oxygen : (int)Element.Vacuum;
                    mass[idx] = grid[idx] == (int)Element.Vacuum ? 0f : 0.3f;
                    temperature[idx] = 250f + rng.Next(-20, 10);
                }
            }
        }

        // Carve out a starting biome pocket in the center
        for (var y = 25; y < 40; y++) {
            for (var x = 20; x < 44; x++) {
                var idx = y * Width + x;
                grid[idx] = (int)Element.Oxygen;
                mass[idx] = 1.8f + (float)(rng.NextDouble() * 0.5);
                temperature[idx] = 295f + (float)(rng.NextDouble() * 3);
            }
        }

        // Add a water pool
        for (var y = 25; y < 28; y++) {
            for (var x = 25; x < 35; x++) {
                var idx = y * Width + x;
                grid[idx] = (int)Element.Water;
                mass[idx] = 800f + rng.Next(0, 200);
                temperature[idx] = 293f;
            }
        }
    }

    private void SpawnEntities() {
        // Duplicants in the starting area
        entities.Add(new EntityData("duplicant", "Meep", 30, 32, "Idle"));
        entities.Add(new EntityData("duplicant", "Bubbles", 33, 34, "Move"));
        entities.Add(new EntityData("duplicant", "Stinky", 28, 30, "Dig"));

        // Buildings
        entities.Add(new EntityData("building", "Printing Pod", 31, 30, "Active"));
        entities.Add(new EntityData("building", "Manual Generator", 35, 30, "Idle"));
        entities.Add(new EntityData("building", "Oxygen Diffuser", 27, 30, "Active"));
        entities.Add(new EntityData("building", "Research Station", 38, 30, "Idle"));
        entities.Add(new EntityData("building", "Outhouse", 24, 30, "Active"));
        entities.Add(new EntityData("building", "Ladder", 31, 31, "Active"));
        entities.Add(new EntityData("building", "Ladder", 31, 32, "Active"));
        entities.Add(new EntityData("building", "Ladder", 31, 33, "Active"));
    }

    public object GetWorldSnapshot() {
        tick++;
        return new {
            width = Width,
            height = Height,
            tick,
            cells = GetCellData()
        };
    }

    private object[] GetCellData() {
        var cells = new object[Width * Height];
        for (var i = 0; i < cells.Length; i++) {
            cells[i] = new {
                element = grid[i],
                temperature = Math.Round(temperature[i], 1),
                mass = Math.Round(mass[i], 2)
            };
        }
        return cells;
    }

    public object GetEntities() {
        return new {
            tick,
            entities = entities.Select(e => new {
                type = e.Type,
                name = e.Name,
                x = e.X,
                y = e.Y,
                state = e.State
            }).ToArray()
        };
    }

    public object GetGameState() {
        return new {
            tick,
            cycle = tick / 600 + 1,
            speed = 1,
            paused = false,
            worldWidth = Width,
            worldHeight = Height,
            duplicantCount = entities.Count(e => e.Type == "duplicant"),
            buildingCount = entities.Count(e => e.Type == "building")
        };
    }

    private class EntityData {
        public string Type { get; }
        public string Name { get; }
        public int X { get; set; }
        public int Y { get; set; }
        public string State { get; set; }

        public EntityData(string type, string name, int x, int y, string state) {
            Type = type;
            Name = name;
            X = x;
            Y = y;
            State = state;
        }
    }
}
