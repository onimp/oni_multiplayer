using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Gameplay;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.World;

// Toilet.FlushMultiple is host-authoritative *production*: it bumps FlushesUsed (fill level -> meter + the
// full/clean state machine), consumes the toilet's stored dirt and spawns polluted dirt + germs into its
// storage (see Toilet.FlushMultiple). Because work runs independently on each machine, a client left to run
// it a second time would both double-produce waste/germs (desync) and NRE on a client-spawned toilet whose
// storage/meter aren't fully wired.
//
// So on the CLIENT we suppress it and let the host stay the single source of truth. Rather than defer the
// result to the daily hard-sync (the old behaviour), the HOST now broadcasts SyncToiletFlush after its own
// flush and the client reproduces the same fill level + polluted-dirt output on the same toilet - so the
// meter, the full/clean cycle and the stored waste all track live. The only piece still left to hard-sync is
// the germs added to the *duplicant*, which ride the duplicant-vitals bucket (replicating them here would
// double-count). See SyncToiletFlush for the reproduction details.
[Dependency, UsedImplicitly]
[HarmonyPatch(typeof(Toilet))]
public class ToiletFlushSynchronizer {

    private static IMultiplayerServer server = null!;
    private static MultiplayerGame multiplayer = null!;
    private static ExecutionLevelManager manager = null!;

    public ToiletFlushSynchronizer(IMultiplayerServer server, MultiplayerGame multiplayer, ExecutionLevelManager manager) {
        ToiletFlushSynchronizer.server = server;
        ToiletFlushSynchronizer.multiplayer = multiplayer;
        ToiletFlushSynchronizer.manager = manager;
    }

    // Skip the flush on the client; the host stays the single source of truth and replicates the result via
    // SyncToiletFlush (sent in the postfix below). See ReplicationGate for the bool-prefix semantics (true
    // runs / false skips) and why [RequireMultiplayerMode] is avoided.
    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch(nameof(Toilet.FlushMultiple))]
    private static bool FlushMultiplePrefix() => !ReplicationGate.IsActiveClient(multiplayer, manager);

    // Host: the flush has run and applied its production; tell the client to reproduce it. __1 is the
    // flushCount argument; the client clamps it against its own (synced) FlushesUsed exactly as the host did.
    [HarmonyPostfix, UsedImplicitly]
    [HarmonyPatch(nameof(Toilet.FlushMultiple))]
    private static void FlushMultiplePostfix(Toilet __instance, int __1) {
        if (ReplicationGate.IsActiveHost(multiplayer, manager))
            server.Send(new SyncToiletFlush(__instance.GetReference<Toilet>(), __1));
    }

}
