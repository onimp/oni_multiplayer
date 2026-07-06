using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Screens.Research;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.World;

// Host-authoritative research unlock. Research points accrue independently on each machine (deferred to the
// daily hard-sync), so a tech the host has finished can remain locked on the client mid-cycle. The unlock
// is a single boolean event, so we replicate that directly rather than the point sim: when the host buys a
// tech - Research.CheckBuyResearch flips activeResearch.complete via TechInstance.Purchased() - tell the
// client to grant the same tech (see SyncTechComplete). One message per tech, not per research point.
//
// Completion is fully host-driven: on the CLIENT we skip Research.CheckBuyResearch entirely (the suppression
// prefix below) so the client never buys/advances research on its own clock - SyncTechComplete is the sole
// completion driver there. On the host the original still runs, so the host/postfix path still detects and
// broadcasts each completion. Client research points may still accrue unused; they reconcile at hard-sync.
[Dependency, UsedImplicitly]
[HarmonyPatch(typeof(Research))]
public class ResearchCompletionSynchronizer {

    private static IMultiplayerServer server = null!;
    private static MultiplayerGame multiplayer = null!;
    private static ExecutionLevelManager manager = null!;

    public ResearchCompletionSynchronizer(
        IMultiplayerServer server,
        MultiplayerGame multiplayer,
        ExecutionLevelManager manager
    ) {
        ResearchCompletionSynchronizer.server = server;
        ResearchCompletionSynchronizer.multiplayer = multiplayer;
        ResearchCompletionSynchronizer.manager = manager;
    }

    // One prefix doing two things (kept in a single method so there's no ambiguity about prefix ordering or a
    // Harmony __state clash):
    //   1. Capture the active research iff it's still incomplete, so the postfix can detect the exact tech that
    //      flipped to complete inside this call - CheckBuyResearch advances to the next tech before returning.
    //   2. Client suppression (bool prefix - see ReplicationGate): return false on an active client to SKIP the
    //      original CheckBuyResearch, so the client never buys/advances research on its own; SyncTechComplete
    //      drives every completion there. Returns true (original runs) on the host and outside multiplayer,
    //      including when deps are still null. The postfix always runs but is gated on IsActiveHost, so the
    //      __state it reads on a client is harmless (the skipped original never flips complete).
    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch(nameof(Research.CheckBuyResearch))]
    private static bool CheckBuyResearchPrefix(Research __instance, out TechInstance? __state) {
        var active = __instance.activeResearch;
        __state = active is { complete: false } ? active : null;
        return !ReplicationGate.IsActiveClient(multiplayer, manager);
    }

    [HarmonyPostfix, UsedImplicitly]
    [HarmonyPatch(nameof(Research.CheckBuyResearch))]
    private static void CheckBuyResearchPostfix(TechInstance? __state) {
        if (__state is not { complete: true })
            return;
        if (!ReplicationGate.IsActiveHost(multiplayer, manager))
            return;

        var techId = __state.tech?.Id;
        if (!string.IsNullOrEmpty(techId))
            server.Send(new SyncTechComplete(techId!));
    }

}
