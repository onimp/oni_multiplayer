using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MultiplayerMod.Test.Environment.Unity.Patches.Unity;

[UsedImplicitly]
[HarmonyPatch(typeof(Application))]
public class ApplicationPatch {

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Application), "get_isPlaying")]
    private static IEnumerable<CodeInstruction> Application_get_isPlaying(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldc_I4_1), // true
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Application), "get_streamingAssetsPath")]
    private static IEnumerable<CodeInstruction> Application_get_streamingAssetsPath(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldstr, ""), // ""
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Application), "get_persistentDataPath")]
    private static IEnumerable<CodeInstruction> Application_persistentDataPath(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldstr, ""), // ""
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Application), "get_consoleLogPath")]
    private static IEnumerable<CodeInstruction> Application_consoleLogPath(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldstr, ""), // ""
            new(OpCodes.Ret)
        };
    }

}
