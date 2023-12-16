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
    [HarmonyPatch("get_isPlaying")]
    private static IEnumerable<CodeInstruction> Application_get_isPlaying(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldc_I4_1), // true
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("get_dataPath")]
    private static IEnumerable<CodeInstruction> Application_get_dataPath(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldstr, ""), // ""true""
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("get_streamingAssetsPath")]
    private static IEnumerable<CodeInstruction> Application_get_streamingAssetsPath(
        IEnumerable<CodeInstruction> instructions
    ) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldstr, ""), // ""
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("get_persistentDataPath")]
    private static IEnumerable<CodeInstruction> Application_persistentDataPath(
        IEnumerable<CodeInstruction> instructions
    ) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldstr, ""), // ""
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("get_consoleLogPath")]
    private static IEnumerable<CodeInstruction> Application_consoleLogPath(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldstr, ""), // ""
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("get_platform")]
    private static IEnumerable<CodeInstruction> Application_get_platform(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldc_I4_2), // WindowsPlayer
            new(OpCodes.Ret)
        };
    }

}
