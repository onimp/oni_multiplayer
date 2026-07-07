using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Gameplay;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.World;

// Host-authoritative order pointer for DUPLICANT-OPERATED ComplexFabricators (Rock Crusher, Microbe Musher,
// ...). The fabricate chore is a WorkChore<ComplexFabricatorWorkable>, already replicated + host-gated for
// completion (WorkChoreSynchronizer). What was NOT replicated is which recipe the fabricator is working:
// StartWorkingOrder picks an order only when the fabricator has the ingredients in storage, and the host
// consumes those ingredients the moment it starts (that consumption streams to the client), so the client's
// copy never has the ingredients to start its own order. With no order, ComplexFabricatorWorkable.GetWorkTime
// returns -1 -> the client's work stint "finishes" instantly and completes via the 5s work fallback (out of
// lockstep, log spam: "OnStartWork ... CurrentMachineOrder is null" + "CompleteWorkingOrder called with no
// working order"), and the client machine shows no recipe/visualizer.
//
// Fix: mirror the host's working-order index to the client and suppress the client's own order lifecycle:
//   - Host: on StartWorkingOrder(index) send that index; on CompleteWorkingOrder / CancelWorkingOrder send -1
//     (as a PREFIX, so the "cleared" reaches the client before the immediately-following StartWorkingOrder for
//     the next order - the reliable Gameplay lane preserves that order).
//   - Client: skip StartWorkingOrder / CompleteWorkingOrder / CancelWorkingOrder / CreateChore entirely; the
//     order pointer comes only from SyncFabricatorOrder and the fabricate chore only from replication. This is
//     what keeps the client from double-spawning the product (SpawnOrderProduct lives inside CompleteWorkingOrder)
//     and from creating a second, un-driven fabricate chore once the mirrored index makes HasWorkingOrder true.
//
// Scoped to duplicantOperated fabricators: auto fabricators have no work chore and produce locally via Sim200ms
// on both machines, so touching their order lifecycle is neither needed nor safe here. One consequence we accept:
// because the client no longer runs CompleteWorkingOrder, its recipeQueueCounts aren't decremented on completion,
// so the client's displayed queue count can read high until the next hard-sync reloads the fabricator state.
[Dependency, UsedImplicitly]
[HarmonyPatch(typeof(ComplexFabricator))]
public class ComplexFabricatorSynchronizer {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<ComplexFabricatorSynchronizer>();

    private static IMultiplayerServer server = null!;
    private static MultiplayerGame multiplayer = null!;
    private static ExecutionLevelManager manager = null!;

    // Private order state on ComplexFabricator we mirror on the client (recipe_list is deterministic, built in
    // OnPrefabInit, so its indices are portable across machines).
    private static readonly FieldInfo workingOrderIdxField = AccessTools.Field(typeof(ComplexFabricator), "workingOrderIdx");
    private static readonly FieldInfo lastWorkingRecipeField = AccessTools.Field(typeof(ComplexFabricator), "lastWorkingRecipe");
    private static readonly FieldInfo recipeListField = AccessTools.Field(typeof(ComplexFabricator), "recipe_list");

    public ComplexFabricatorSynchronizer(
        IMultiplayerServer server, MultiplayerGame multiplayer, ExecutionLevelManager manager
    ) {
        ComplexFabricatorSynchronizer.server = server;
        ComplexFabricatorSynchronizer.multiplayer = multiplayer;
        ComplexFabricatorSynchronizer.manager = manager;
    }

    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch("StartWorkingOrder")]
    private static bool StartWorkingOrderPrefix(ComplexFabricator __instance, int index) {
        SendOrder(__instance, index);
        return AllowLocalOrder(__instance);
    }

    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch(nameof(ComplexFabricator.CompleteWorkingOrder))]
    private static bool CompleteWorkingOrderPrefix(ComplexFabricator __instance) {
        SendOrder(__instance, -1);
        return AllowLocalOrder(__instance);
    }

    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch("CancelWorkingOrder")]
    private static bool CancelWorkingOrderPrefix(ComplexFabricator __instance) {
        SendOrder(__instance, -1);
        return AllowLocalOrder(__instance);
    }

    // The fabricate chore is always replicated from the host; the client must never spin up its own once the
    // mirrored index makes HasWorkingOrder true (UpdateChore would otherwise create a second, un-driven chore).
    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch("CreateChore")]
    private static bool CreateChorePrefix(ComplexFabricator __instance) => AllowLocalOrder(__instance);

    // Host -> clients: this duplicant-operated fabricator's working recipe is now `index` (< 0 = no order).
    private static void SendOrder(ComplexFabricator fabricator, int index) {
        if (server == null || !ReplicationGate.IsActiveHost(multiplayer, manager))
            return;
        if (fabricator == null || !fabricator.duplicantOperated || server.Clients.Count == 0)
            return;
        server.Send(new SyncFabricatorOrder(fabricator.gameObject, index));
    }

    // Bool-prefix gate (true = run original, false = skip): only skip the local order lifecycle on an active
    // client, and only for the duplicant-operated fabricators we mirror. Deps still null (patch fired before the
    // session is up) -> IsActiveClient false -> true -> original runs, the safe default. See ReplicationGate.
    private static bool AllowLocalOrder(ComplexFabricator fabricator) =>
        !(ReplicationGate.IsActiveClient(multiplayer, manager) && fabricator != null && fabricator.duplicantOperated);

    // Client apply for SyncFabricatorOrder: mirror the host's order pointer without any of StartWorkingOrder's
    // side effects (ingredient transfer, chore creation, open-order bookkeeping) - just enough state that
    // CurrentWorkingOrder resolves and GetWorkTime returns the real recipe time.
    public static void ApplyOrder(ComplexFabricator fabricator, int recipeIndex) {
        if (fabricator == null || workingOrderIdxField == null)
            return;

        if (recipeIndex < 0) {
            workingOrderIdxField.SetValue(fabricator, -1);
            fabricator.OrderProgress = 0f;
            return;
        }

        if (recipeListField?.GetValue(fabricator) is not ComplexRecipe[] recipes ||
            recipeIndex >= recipes.Length || recipes[recipeIndex] == null) {
            log.Warning($"SyncFabricatorOrder: recipe index {recipeIndex} out of range on this client");
            return;
        }

        workingOrderIdxField.SetValue(fabricator, recipeIndex);
        lastWorkingRecipeField?.SetValue(fabricator, recipes[recipeIndex].id);
        fabricator.OrderProgress = 0f;
    }

}
