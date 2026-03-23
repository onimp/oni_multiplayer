// Partial file: RealWorldState.GameStateDto.cs
// Contains only BuildGameStateDto() — no game-object references so this file
// can be compiled independently by DedicatedServer.Test without game DLLs.

namespace DedicatedServer.Game;

public partial class RealWorldState {

    /// <summary>
    /// Builds the game-state DTO as an anonymous object suitable for JSON
    /// serialisation via Newtonsoft.Json.
    ///
    /// Extracted as <c>internal static</c> so unit tests can verify the JSON
    /// shape — including the <c>bootErrorCount</c> field — without requiring a
    /// running game instance.  All parameters are BCL primitives; no game types.
    /// </summary>
    internal static object BuildGameStateDto(
        int    tick,
        int    cycle,
        int    speed,
        bool   paused,
        int    worldWidth,
        int    worldHeight,
        int    duplicantCount,
        int    buildingCount,
        int    entityCount,
        string source,
        int    serverUps,
        float  cycleTime,
        bool   isNight,
        int    bootErrorCount)
    {
        return new {
            tick,
            cycle,
            speed,
            paused,
            worldWidth,
            worldHeight,
            duplicantCount,
            buildingCount,
            entityCount,
            source,
            serverUps,
            cycleTime,
            isNight,
            bootErrorCount,
        };
    }
}
