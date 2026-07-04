using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime.Context;

namespace MultiplayerMod.Multiplayer.World;

// Toilet.FlushMultiple is host-authoritative *production*: it consumes the toilet's stored dirt, spawns
// polluted dirt + germs into its storage, and adds germs to the duplicant (see Toilet.FlushMultiple).
// Because work runs independently on each machine, the client executes it a second time - which both
// double-produces waste/germs (desync) and NREs on a client-spawned toilet whose storage/meter aren't
// fully wired (crash: "Toilet.FlushMultiple ... NullReferenceException").
//
// Suppress it on the client. The host remains the single source of truth; the toilet's fill level and
// the polluted-dirt output are meant to be result-replicated (chore-result sync, roadmap #2), and the
// duplicant's germ load is covered by the duplicant germ/effects sync (roadmap #1). Skipping the flush
// does not stall the duplicant - the work state machine still completes; only the side effects are cut.
[Dependency, UsedImplicitly]
[HarmonyPatch(typeof(Toilet))]
public class ToiletFlushSynchronizer {

    private static MultiplayerGame multiplayer = null!;
    private static ExecutionLevelManager manager = null!;

    public ToiletFlushSynchronizer(MultiplayerGame multiplayer, ExecutionLevelManager manager) {
        ToiletFlushSynchronizer.multiplayer = multiplayer;
        ToiletFlushSynchronizer.manager = manager;
    }

    // Skip the flush on the client; the host stays the single source of truth. See ReplicationGate for the
    // bool-prefix semantics (true runs / false skips) and why [RequireMultiplayerMode] is avoided.
    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch(nameof(Toilet.FlushMultiple))]
    private static bool FlushMultiplePrefix() => !ReplicationGate.IsActiveClient(multiplayer, manager);

}
