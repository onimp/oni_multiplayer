using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Gameplay;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.World;

// Host-authoritative terrain removal (digging, and any other cell destruction that funnels through
// WorldDamage.DestroyCell). Each side runs the dig simulation on its own, so a starved client falls
// behind and leaves blocks solid that the host has already removed. Rather than trying to reproduce
// the client's work economy deterministically, we let the host be the single source of truth: whenever
// it removes a solid cell, broadcast it so the client removes the same cell (see SyncDugCell).
[Dependency, UsedImplicitly]
[HarmonyPatch(typeof(WorldDamage))]
public class DigSynchronizer {

    private static IMultiplayerServer server = null!;
    private static MultiplayerGame multiplayer = null!;
    private static ExecutionLevelManager manager = null!;

    public DigSynchronizer(IMultiplayerServer server, MultiplayerGame multiplayer, ExecutionLevelManager manager) {
        DigSynchronizer.server = server;
        DigSynchronizer.multiplayer = multiplayer;
        DigSynchronizer.manager = manager;
    }

    // DestroyCell only acts when the cell is currently solid, so we sample Grid.Solid in a prefix (it is
    // still true here) to send the command exactly when the host is about to remove a cell. Runs on both
    // sides but only the host broadcasts; on the client the command-driven DestroyCell re-enters this
    // prefix as a non-host no-op, so there is no echo.
    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch(nameof(WorldDamage.DestroyCell))]
    private static void DestroyCellPrefix(int cell) {
        // DestroyCell can fire during world gen/load before the DI container has built this component; the
        // gate treats null deps as an inactive session (see ReplicationGate).
        if (server == null || !ReplicationGate.IsActiveHost(multiplayer, manager))
            return;
        if (Grid.IsValidCell(cell) && Grid.Solid[cell])
            server.Send(new SyncDugCell(cell));
    }

}
