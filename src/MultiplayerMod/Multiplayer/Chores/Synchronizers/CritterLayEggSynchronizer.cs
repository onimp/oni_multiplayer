using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Gameplay;
using MultiplayerMod.Network;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Chores.Synchronizers;

// Host-authoritative egg laying. FertilityMonitor.LayEgg picks an egg via a non-deterministic breeding roll
// (EggBreedingRoll) and KInstantiates it into the private `egg` field; ShowEgg positions + activates it. The
// two machines would roll different egg types, so the client must NOT lay its own - it suppresses both
// callbacks and materializes the host's egg (same tag, same shared id) via SyncSpawnCritter.
//
// Client suppresses LayEgg AND ShowEgg: ShowEgg NREs on a null `egg` if LayEgg was skipped, so both go.
// Host reads the freshly-laid egg from the `___egg` field in the LayEgg postfix (ShowEgg nulls it later) and
// replicates it. Position is the layer's cell; ShowEgg's small snapto nudge on the host is cosmetic and
// reconciles at the hard-sync. See ReplicationGate for the bool-prefix semantics (true=run / false=skip).
[Dependency, UsedImplicitly]
[HarmonyPatch(typeof(FertilityMonitor.Instance))]
public class CritterLayEggSynchronizer {

    private static IMultiplayerServer server = null!;
    private static MultiplayerGame multiplayer = null!;
    private static ExecutionLevelManager manager = null!;

    public CritterLayEggSynchronizer(IMultiplayerServer server, MultiplayerGame multiplayer, ExecutionLevelManager manager) {
        CritterLayEggSynchronizer.server = server;
        CritterLayEggSynchronizer.multiplayer = multiplayer;
        CritterLayEggSynchronizer.manager = manager;
    }

    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch(nameof(FertilityMonitor.Instance.LayEgg))]
    private static bool LayEggPrefix() => !ReplicationGate.IsActiveClient(multiplayer, manager);

    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch(nameof(FertilityMonitor.Instance.ShowEgg))]
    private static bool ShowEggPrefix() => !ReplicationGate.IsActiveClient(multiplayer, manager);

    // ___egg is the private FertilityMonitor.Instance.egg field, still set here (ShowEgg clears it later).
    [HarmonyPostfix, UsedImplicitly]
    [HarmonyPatch(nameof(FertilityMonitor.Instance.LayEgg))]
    private static void LayEggPostfix(GameObject ___egg) {
        if (server == null || !ReplicationGate.IsActiveHost(multiplayer, manager))
            return;
        if (___egg == null)
            return;
        var cell = Grid.PosToCell(___egg.transform.GetPosition());
        if (!Grid.IsValidCell(cell))
            return;
        server.Send(SyncSpawnCritter.Create(___egg, cell));
    }

}
