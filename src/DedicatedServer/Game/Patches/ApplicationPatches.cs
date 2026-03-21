using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace DedicatedServer.Game.Patches;

/// <summary>
/// Patches for UnityEngine.Application — paths, platform, isPlaying.
/// </summary>
[HarmonyPatch(typeof(Application))]
public static class ApplicationPatches {

    [HarmonyTranspiler]
    [HarmonyPatch("get_isPlaying")]
    static IEnumerable<CodeInstruction> get_isPlaying(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldc_I4_1),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("get_streamingAssetsPath")]
    static IEnumerable<CodeInstruction> get_streamingAssetsPath(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetStreamingAssetsPath)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("get_dataPath")]
    static IEnumerable<CodeInstruction> get_dataPath(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetDataPath)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("get_persistentDataPath")]
    static IEnumerable<CodeInstruction> get_persistentDataPath(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetPersistentDataPath)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("get_consoleLogPath")]
    static IEnumerable<CodeInstruction> get_consoleLogPath(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetConsoleLogPath)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("get_platform")]
    static IEnumerable<CodeInstruction> get_platform(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetPlatform)),
            new(OpCodes.Ret)
        };
    }
}
