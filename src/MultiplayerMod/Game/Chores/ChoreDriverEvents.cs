using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Multiplayer.CoreOperations;

namespace MultiplayerMod.Game.Chores;

[UsedImplicitly]
[HarmonyPatch(typeof(ChoreDriver))]
public static class ChoreDriverEvents {

    public delegate void ChoreSetEventHandler(
        ChoreDriver driver,
        Chore? previousChore,
        ref Chore.Precondition.Context context
    );

    public static event ChoreSetEventHandler? ChoreSet;

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ChoreDriver.SetChore))]
    [RequireMultiplayerMode(MultiplayerMode.Host)]
    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    private static void SetChorePrefix(ChoreDriver __instance, out Chore? __state) {
        __state = __instance.GetCurrentChore();
    }

    [UsedImplicitly]
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ChoreDriver.SetChore))]
    [RequireMultiplayerMode(MultiplayerMode.Host)]
    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    private static void SetChorePostfix(
        ChoreDriver __instance,
        Chore? __state,
        ref Chore.Precondition.Context context
    ) {
        if (__state == context.chore)
            return;
        ChoreSet?.Invoke(__instance, __state, ref context);
    }

}
