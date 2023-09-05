using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Test.Environment.Unity.Patches;

[UsedImplicitly]
[HarmonyPatch(typeof(SpeedControlScreen))]
public static class SpeedControlScreenPatch {

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(SpeedControlScreen.Pause))]
    private static IEnumerable<CodeInstruction> Pause(IEnumerable<CodeInstruction> instructions) {
        using var source = instructions.GetEnumerator();
        var result = new List<CodeInstruction>();
        result.AddConditional(
            source,
            it => it.StoresField(AccessTools.Field(typeof(SpeedControlScreen), "pauseCount"))
        );
        result.Add(new CodeInstruction(OpCodes.Ret));
        return result;
    }

}
