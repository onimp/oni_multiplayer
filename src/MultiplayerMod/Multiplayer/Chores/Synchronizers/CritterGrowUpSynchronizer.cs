using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Gameplay;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Chores.Synchronizers;

// Host-authoritative grow-up. BabyMonitor.SpawnAdult KInstantiates the adult (def.adultPrefab) AND deletes the
// baby in one method, on the baby's independent age timer. The client suppresses the whole method and lets the
// host drive the swap: replicate the adult spawn (SyncSpawnCritter) then the baby despawn (SyncDespawnEntity).
//
// The adult lands in a local variable and the baby is destroyed by the time a postfix runs, so the host reads
// the baby's id + cell + the adult tag in the PREFIX (baby still alive), then in the POSTFIX finds the newly
// spawned adult structurally (FindUnregisteredCritter) and replicates. Spawn-then-despawn order avoids a frame
// where the critter is missing on the client. Single-threaded main thread, non-reentrant -> static hand-off is
// safe. See ReplicationGate for the bool-prefix semantics (true=run / false=skip).
[Dependency, UsedImplicitly]
[HarmonyPatch(typeof(BabyMonitor.Instance))]
public class CritterGrowUpSynchronizer {

    private static IMultiplayerServer server = null!;
    private static MultiplayerGame multiplayer = null!;
    private static ExecutionLevelManager manager = null!;

    private static MultiplayerId? pendingBabyId;
    private static int pendingCell;
    private static Tag pendingAdultTag;

    public CritterGrowUpSynchronizer(IMultiplayerServer server, MultiplayerGame multiplayer, ExecutionLevelManager manager) {
        CritterGrowUpSynchronizer.server = server;
        CritterGrowUpSynchronizer.multiplayer = multiplayer;
        CritterGrowUpSynchronizer.manager = manager;
    }

    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch(nameof(BabyMonitor.Instance.SpawnAdult))]
    private static bool SpawnAdultPrefix(BabyMonitor.Instance __instance) {
        pendingBabyId = null;
        if (ReplicationGate.IsActiveClient(multiplayer, manager))
            return false; // client keeps its baby until the host despawns it and spawns the adult
        if (server != null && ReplicationGate.IsActiveHost(multiplayer, manager)) {
            pendingBabyId = __instance.gameObject.GetComponent<MultiplayerInstance>()?.Id;
            pendingCell = Grid.PosToCell(__instance.gameObject.transform.GetPosition());
            pendingAdultTag = __instance.def.adultPrefab;
        }
        return true;
    }

    [HarmonyPostfix, UsedImplicitly]
    [HarmonyPatch(nameof(BabyMonitor.Instance.SpawnAdult))]
    private static void SpawnAdultPostfix() {
        if (server == null || !ReplicationGate.IsActiveHost(multiplayer, manager))
            return;
        if (Grid.IsValidCell(pendingCell)) {
            var adult = CritterLifecycleSupport.FindUnregisteredCritter(pendingAdultTag, pendingCell);
            if (adult != null)
                server.Send(SyncSpawnCritter.Create(adult, pendingCell));
            if (pendingBabyId != null)
                server.Send(new SyncDespawnEntity(pendingBabyId));
        }
        pendingBabyId = null;
    }

}
