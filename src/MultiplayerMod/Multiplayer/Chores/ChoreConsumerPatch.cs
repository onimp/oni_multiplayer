using HarmonyLib;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.CoreOperations;

namespace MultiplayerMod.Multiplayer.Chores;

[HarmonyPatch(typeof(ChoreConsumer))]
public class ChoreConsumerPatch {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<ChoreConsumerPatch>();

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ChoreConsumer.FindNextChore))]
    [RequireMultiplayerMode(MultiplayerMode.Client)]
    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    public static void ChoreConsumerFindNextChore(ref Chore.Precondition.Context out_context, bool __result) {
        if (!__result)
            return;

        var pooledChore = MultiplayerHostChores.FindPooledChore(ref out_context);
        if (pooledChore == null)
            return;

        var driver = out_context.consumerState.choreDriver;
        log.Debug($"Chore driver {driver.GetProperName()} picked up a pooled chore {pooledChore}");

        MultiplayerHostChores.RemovePooledChore(pooledChore);
    }


}
