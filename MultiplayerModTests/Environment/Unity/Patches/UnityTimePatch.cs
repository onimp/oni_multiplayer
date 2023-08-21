using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MultiplayerModTests.Environment.Unity.Patches;

[UsedImplicitly]
[HarmonyPatch(typeof(Time))]
public class UnityTimePatch {

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Time), "get_frameCount")]
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
