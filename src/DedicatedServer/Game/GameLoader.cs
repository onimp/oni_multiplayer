using System;

namespace DedicatedServer.Game;

/// <summary>
/// Entry point for ONI Dedicated Server boot sequence.
/// </summary>
public class GameLoader {

    public ResourceLoader Resources { get; private set; }
    public WorldBuilder World { get; private set; }

    public void Boot() {
        Console.WriteLine("[GameLoader] Booting...");
        Resources = new ResourceLoader();
        Resources.Load();
        World = new WorldBuilder();
        World.Create(Resources);
        Console.WriteLine("[GameLoader] Boot complete.");
    }

    public void Shutdown() => World?.Shutdown();
}
