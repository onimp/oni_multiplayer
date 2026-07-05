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

    // Capture the active research iff it's still incomplete, so the postfix can detect the exact tech that
    // flipped to complete inside this call - CheckBuyResearch advances to the next tech before returning.
    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch(nameof(Research.CheckBuyResearch))]
    private static void CheckBuyResearchPrefix(Research __instance, out TechInstance? __state) {
        var active = __instance.activeResearch;
        __state = active is { complete: false } ? active : null;
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
