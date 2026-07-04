using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Gameplay;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.World;

// Mopping is host-authoritative *production* with two side effects, both funneled through the static
// Moppable.MopCell (see Moppable.MopTick -> MopCell):
//   1. SimMessages.ConsumeMass removes liquid mass from the cell (the "cell result").
//   2. the mass-consumed callback OnCellMopped bottles that liquid into a SubstanceChunk pickupable on
//      the ground (the "entity result").
// Because work runs independently on each machine, the client re-runs MopCell on its own copy of the
// cell: its ConsumeMass fights the fluid stream (SyncSimCells) and it double-bottles / mis-bottles the
// pickupable (the materials economy is not synced).
//
// Suppress MopCell on the client. Cutting this one seam removes both side effects at once (no ConsumeMass
// and, because the mass-consumed callback is only registered inside MopCell, no OnCellMopped -> no bottle).
// The liquid removal still reaches the client via the fluid stream, and the Moppable self-destructs there
// once IsThereLiquid() sees the streamed cell go dry (Refresh -> TryDestroy), so its lifecycle converges
// without the client running any mop labor. The bottled SubstanceChunk is a pickupable/economy result
// (task #2 track B) and is simply absent on the client until it can be spawn-replicated / hard-synced.
[Dependency, UsedImplicitly]
[HarmonyPatch(typeof(Moppable))]
public class MopSynchronizer {

    private static IMultiplayerServer server = null!;
    private static MultiplayerGame multiplayer = null!;
    private static ExecutionLevelManager manager = null!;

    public MopSynchronizer(IMultiplayerServer server, MultiplayerGame multiplayer, ExecutionLevelManager manager) {
        MopSynchronizer.server = server;
        MopSynchronizer.multiplayer = multiplayer;
        MopSynchronizer.manager = manager;
    }

    // Suppress the client's mop production (mass-consume + bottle); the host stays authoritative. See
    // ReplicationGate for the bool-prefix semantics (true runs / false skips).
    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch(nameof(Moppable.MopCell))]
    private static bool MopCellPrefix() => !ReplicationGate.IsActiveClient(multiplayer, manager);

    // Phase B — host-authoritative bottle replication. On the host, MopCell ran and its mass-consumed
    // callback OnCellMopped bottled the liquid into a local SubstanceChunk. Mirror that spawn to clients
    // (which had MopCell suppressed, so they produced no bottle) via SyncSpawnPickupable. Runs on both
    // sides but only the host broadcasts; the client never reaches OnCellMopped anyway (its MopCell is
    // suppressed), and the host-mode gate is a second guard. The mass_cb_info carries the exact element /
    // mass / temperature / disease the host bottled, so the client's chunk matches.
    [HarmonyPostfix, UsedImplicitly]
    [HarmonyPatch("OnCellMopped")]
    private static void OnCellMoppedPostfix(Moppable __instance, Sim.MassConsumedCallback __0) {
        if (server == null || !ReplicationGate.IsActiveHost(multiplayer, manager))
            return;
        if (__instance == null || __0.mass <= 0f)
            return;
        server.Send(SyncSpawnPickupable.LiquidChunk(
            Grid.PosToCell(__instance),
            __0.elemIdx,
            __0.mass,
            __0.temperature,
            __0.diseaseIdx,
            __0.diseaseCount
        ));
    }

}
