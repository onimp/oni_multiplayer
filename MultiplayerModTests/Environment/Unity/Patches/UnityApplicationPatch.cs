using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MultiplayerModTests.Environment.Unity.Patches;

[UsedImplicitly]
[HarmonyPatch(typeof(Application))]
public class UnityApplicationPatch {

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Application), "get_isPlaying")]
    private static IEnumerable<CodeInstruction> Application_get_isPlaying(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldc_I4_1), // true
            new(OpCodes.Ret)
        };
    }

}
