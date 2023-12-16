using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MultiplayerMod.Test.Environment.Unity.Patches.Unity;

[UsedImplicitly]
[HarmonyPatch(typeof(Time))]
public class TimePatch {

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("get_time")]
    private static IEnumerable<CodeInstruction> Time_get_time(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(
                OpCodes.Call,
                AccessTools.PropertyGetter(typeof(UnityTestRuntime), nameof(UnityTestRuntime.Time))
            ),
            new(OpCodes.Ret)
        };
   }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("get_frameCount")]
    private static IEnumerable<CodeInstruction> Time_get_frameCount(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(
                OpCodes.Call,
                AccessTools.PropertyGetter(typeof(UnityTestRuntime), nameof(UnityTestRuntime.FrameCount))
            ),
            new(OpCodes.Ret)
        };
    }
}
