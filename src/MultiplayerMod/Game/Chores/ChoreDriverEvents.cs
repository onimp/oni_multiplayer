using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Multiplayer.CoreOperations;

namespace MultiplayerMod.Game.Chores;

[UsedImplicitly]
[HarmonyPatch(typeof(ChoreDriver))]
public static class ChoreDriverEvents {

    public delegate void ChoreSettingEventHandler(
        ChoreDriver driver,
        Chore? previousChore,
        ref Chore.Precondition.Context context
    );

    public static event ChoreSettingEventHandler? ChoreSetting;

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(ChoreDriver.SetChore))]
    private static IEnumerable<CodeInstruction> ChoreDriverSetChoreTranspiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator
    ) {
        using var source = instructions.GetEnumerator();
        var result = new List<CodeInstruction>();

        var contextField = AccessTools.Field(typeof(ChoreDriver), nameof(ChoreDriver.context));
        var beforeChoreSetMethod = AccessTools.Method(typeof(ChoreDriverEvents), nameof(BeforeChoreSet));

        result.AddConditional(source, it => it.StoresField(contextField));

        // Add BeforeChoreSet call right before next chore property set, because the actual set can cause chore cleanup
        // and break objects index synchronized state (e.g. when new chore is being sent to the clients there will be no
        // chore in the index).
        result.Add(new CodeInstruction(OpCodes.Ldarg_0)); // this
        result.Add(new CodeInstruction(OpCodes.Ldloc_0)); // currentChore
        result.Add(new CodeInstruction(OpCodes.Ldarga, 1)); // ref context
        result.Add(new CodeInstruction(OpCodes.Call, beforeChoreSetMethod));

        result.AddConditional(source, _ => false);

        return result;
    }

    [RequireMultiplayerMode(MultiplayerMode.Host)]
    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    private static void BeforeChoreSet(
        ChoreDriver driver,
        Chore? previousChore,
        ref Chore.Precondition.Context context
    ) => ChoreSetting?.Invoke(driver, previousChore, ref context);

}
