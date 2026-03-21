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
        // Must be first — patches Unity InternalCalls before ANY game assembly code runs.
        // Without this, ElementLoader..cctor fires on first touch of ElementLoader and
        // crashes on Application.get_streamingAssetsPath() (unresolved native binding).
        try {
            UnityRuntime.Install();
        } catch (Exception ex) {
            Console.WriteLine($"[GameLoader] UnityRuntime.Install failed: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
        Resources = new ResourceLoader();
        Resources.Load();
        World = new WorldBuilder();
        World.Create(Resources);
        Console.WriteLine("[GameLoader] Boot complete.");
    }

    public void Shutdown() => World?.Shutdown();
}
