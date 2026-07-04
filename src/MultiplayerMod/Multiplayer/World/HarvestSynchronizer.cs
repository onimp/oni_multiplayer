using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Gameplay;
using MultiplayerMod.Network;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.World;

// Harvesting is host-authoritative *production*: Crop.SpawnSomeFruit KInstantiates a food pickupable at
// the plant (amount from the YieldAmount attribute, plus a non-deterministic mutation roll) and reports
// calories. Because work runs independently on each machine, the client re-runs the harvest on its own
// copy of the plant and either double-spawns food, spawns a different amount (its plant may be at a
// different growth stage), or NREs on an invalid state - and the mutation roll never matches the host.
//
// Suppress SpawnSomeFruit on the client. The host stays the single source of truth for the produced crop;
// the fruit is a pickupable/economy result (task #2 track B) and is absent on the client until it can be
// spawn-replicated / hard-synced. Only the actual instantiate is cut - Crop.SpawnConfiguredFruit still
// fires its own harvest trigger, so the plant's post-harvest bookkeeping/reset runs on the client.
[Dependency, UsedImplicitly]
[HarmonyPatch(typeof(Crop))]
public class HarvestSynchronizer {

    private static IMultiplayerServer server = null!;
    private static MultiplayerGame multiplayer = null!;
    private static ExecutionLevelManager manager = null!;

    public HarvestSynchronizer(IMultiplayerServer server, MultiplayerGame multiplayer, ExecutionLevelManager manager) {
        HarvestSynchronizer.server = server;
        HarvestSynchronizer.multiplayer = multiplayer;
        HarvestSynchronizer.manager = manager;
    }

    // Suppress the crop spawn on the client; the host stays authoritative. See ReplicationGate for the
    // bool-prefix semantics (true runs / false skips).
    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch(nameof(Crop.SpawnSomeFruit))]
    private static bool SpawnSomeFruitPrefix() => !ReplicationGate.IsActiveClient(multiplayer, manager);

    // Phase B — host-authoritative crop replication. On the host the original ran and KInstantiated the
    // food pickupable; mirror that spawn to clients (which had SpawnSomeFruit suppressed) via
    // SyncSpawnPickupable. Harmony still runs postfixes when a prefix skipped the original, so this fires
    // on the client too - the host-mode gate is what keeps it host-only. Mirrors SpawnSomeFruit's own
    // placement (transform + cropSpawnOffset) and temperature (the plant's PrimaryElement). The
    // non-deterministic mutation roll is NOT replicated in this cut (rare; hard-sync backstops genetics).
    [HarmonyPostfix, UsedImplicitly]
    [HarmonyPatch(nameof(Crop.SpawnSomeFruit))]
    private static void SpawnSomeFruitPostfix(Crop __instance, Tag __0, float __1) {
        if (server == null || !ReplicationGate.IsActiveHost(multiplayer, manager))
            return;
        if (__instance == null)
            return;
        var position = __instance.transform.GetPosition() + __instance.cropSpawnOffset;
        var cell = Grid.PosToCell(position);
        if (!Grid.IsValidCell(cell))
            return;
        var primaryElement = __instance.GetComponent<PrimaryElement>();
        var temperature = primaryElement != null ? primaryElement.Temperature : 0f;
        server.Send(SyncSpawnPickupable.Prefab(
            __0.Name,
            cell,
            __1,
            temperature,
            byte.MaxValue,
            0
        ));
    }

}
