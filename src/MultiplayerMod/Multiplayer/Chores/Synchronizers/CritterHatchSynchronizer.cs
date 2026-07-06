using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Gameplay;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Chores.Synchronizers;

// Host-authoritative hatching. IncubationMonitor.SpawnBaby KInstantiates the baby (def.spawnedCreature) and
// DeleteSelf consumes the egg; both run on the egg's own independent incubation timer, so the client must not
// run them itself. The client suppresses both, and the host replicates a baby spawn (SyncSpawnCritter) plus an
// egg despawn (SyncDespawnEntity, keyed by the egg's shared id).
//
// SpawnBaby drops the baby into a local variable, so the host identifies it structurally after the fact
// (CritterLifecycleSupport.FindUnregisteredCritter). DeleteSelf is the single delete site for BOTH the normal
// hatch (hatching_pst.Exit) and the non-viable path (not_viable.Exit), so despawning from its prefix covers
// both; the id must be read before the original destroys the egg. See ReplicationGate for bool-prefix rules.
[Dependency, UsedImplicitly]
[HarmonyPatch(typeof(IncubationMonitor))]
public class CritterHatchSynchronizer {

    private static IMultiplayerServer server = null!;
    private static MultiplayerGame multiplayer = null!;
    private static ExecutionLevelManager manager = null!;

    public CritterHatchSynchronizer(IMultiplayerServer server, MultiplayerGame multiplayer, ExecutionLevelManager manager) {
        CritterHatchSynchronizer.server = server;
        CritterHatchSynchronizer.multiplayer = multiplayer;
        CritterHatchSynchronizer.manager = manager;
    }

    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch("SpawnBaby")]
    private static bool SpawnBabyPrefix() => !ReplicationGate.IsActiveClient(multiplayer, manager);

    [HarmonyPostfix, UsedImplicitly]
    [HarmonyPatch("SpawnBaby")]
    private static void SpawnBabyPostfix(IncubationMonitor.Instance smi) {
        if (server == null || !ReplicationGate.IsActiveHost(multiplayer, manager))
            return;
        var cell = Grid.PosToCell(smi.gameObject.transform.GetPosition());
        if (!Grid.IsValidCell(cell))
            return;
        var baby = CritterLifecycleSupport.FindUnregisteredCritter(smi.def.spawnedCreature, cell);
        if (baby != null)
            server.Send(SyncSpawnCritter.Create(baby, cell));
    }

    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch("DeleteSelf")]
    private static bool DeleteSelfPrefix(IncubationMonitor.Instance smi) {
        if (ReplicationGate.IsActiveClient(multiplayer, manager))
            return false; // host drives egg removal via SyncDespawnEntity; keep the client egg until then
        if (server != null && ReplicationGate.IsActiveHost(multiplayer, manager)) {
            var eggId = smi.gameObject.GetComponent<MultiplayerInstance>()?.Id;
            if (eggId != null)
                server.Send(new SyncDespawnEntity(eggId));
        }
        return true;
    }

}
