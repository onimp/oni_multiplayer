using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Gameplay;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.World;

// Host-authoritative Operational.IsActive (drives every machine's working animation / operating status /
// uptime). See SyncBuildingActive for why the client can't derive it locally. Two halves:
//
//   * Host: a postfix on Operational.SetActive broadcasts the new IsActive the instant it really changes,
//     so the client's animation reacts immediately (the periodic OperationalSynchronizer re-stream is only
//     the convergence safety net for dropped commands / buildings already active before this client joined).
//
//   * Client: a prefix on Operational.SetActive suppresses the building's OWN active toggles. The client
//     runs each machine's logic locally but starves it (no replicated input), so without this its logic
//     would keep flipping IsActive back off and fight the synced value, flickering the animation. Only the
//     host's value (applied through ApplyHostValue) is allowed through, making active state 100% host-driven.
[Dependency, UsedImplicitly]
public class OperationalActiveSynchronizer {

    private static IMultiplayerServer server = null!;
    private static MultiplayerGame multiplayer = null!;
    private static ExecutionLevelManager manager = null!;

    // True only for the duration of a host-driven SetActive (via ApplyHostValue), so the client suppression
    // prefix lets exactly that call through. Runs entirely on the Unity main thread, so no locking needed.
    private static bool applyingHostValue;

    public OperationalActiveSynchronizer(
        IMultiplayerServer server,
        MultiplayerGame multiplayer,
        ExecutionLevelManager manager
    ) {
        OperationalActiveSynchronizer.server = server;
        OperationalActiveSynchronizer.multiplayer = multiplayer;
        OperationalActiveSynchronizer.manager = manager;
    }

    // Apply the host's authoritative active state on the client, bypassing the suppression prefix. Called by
    // SyncBuildingActive (event-driven change) and the periodic OperationalSynchronizer re-stream.
    public static void ApplyHostValue(Operational operational, bool active) {
        applyingHostValue = true;
        try {
            operational.SetActive(active);
        } finally {
            applyingHostValue = false;
        }
    }

    [HarmonyPatch(typeof(Operational), nameof(Operational.SetActive))]
    private static class SetActivePatch {

        // bool prefix: return true to RUN the original, false to SKIP it (see ReplicationGate doc). Capture
        // the pre-call IsActive for the host postfix's change test regardless of which branch we take.
        [HarmonyPrefix, UsedImplicitly]
        private static bool Prefix(Operational __instance, ref bool __state) {
            __state = __instance.IsActive;
            // Client: only the host may change active state. Deps still null (pre-session) -> IsActiveClient
            // false -> returns true -> original runs, the safe default.
            return applyingHostValue || !ReplicationGate.IsActiveClient(multiplayer, manager);
        }

        [HarmonyPostfix, UsedImplicitly]
        private static void Postfix(Operational __instance, bool __state) {
            if (!ReplicationGate.IsActiveHost(multiplayer, manager))
                return;
            if (__state == __instance.IsActive)
                return; // SetActive is a no-op unless the value actually changed
            if (server.Clients.Count == 0)
                return;
            server.Send(new SyncBuildingActive(__instance.GetReference<Operational>(), __instance.IsActive));
        }

    }

}
