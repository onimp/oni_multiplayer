using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace DedicatedServer.Game.Patches;

/// <summary>
/// Patches for Behaviour, MonoBehaviour, ScriptableObject, TextAsset, SystemInfo, Debug.
/// </summary>

[HarmonyPatch(typeof(Behaviour))]
public static class BehaviourPatches {

    [HarmonyTranspiler]
    [HarmonyPatch("get_isActiveAndEnabled")]
    static IEnumerable<CodeInstruction> get_isActiveAndEnabled(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldc_I4_1),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("get_enabled")]
    static IEnumerable<CodeInstruction> get_enabled(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldc_I4_1),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("set_enabled")]
    static IEnumerable<CodeInstruction> set_enabled(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> { new(OpCodes.Ret) };
    }
}

[HarmonyPatch(typeof(MonoBehaviour))]
public static class MonoBehaviourPatches {

    [HarmonyPrefix]
    [HarmonyPatch(nameof(MonoBehaviour.StartCoroutine), typeof(IEnumerator))]
    static bool StartCoroutine() => false; // Skip coroutines in headless

    [HarmonyTranspiler]
    [HarmonyPatch("IsObjectMonoBehaviour")]
    static IEnumerable<CodeInstruction> IsObjectMonoBehaviour(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldc_I4_1),
            new(OpCodes.Ret)
        };
    }
}

[HarmonyPatch(typeof(ScriptableObject))]
public static class ScriptableObjectPatches {

    [HarmonyPostfix]
    [HarmonyPatch(MethodType.Constructor)]
    static void ScriptableObject_Constructor(ScriptableObject __instance) {
        UnityRuntime.CreateScriptableObject(__instance);
    }

    [HarmonyTranspiler]
    [HarmonyPatch("CreateScriptableObjectInstanceFromType")]
    static IEnumerable<CodeInstruction> CreateScriptableObjectInstanceFromType(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.CreateScriptableObjectInstanceFromType)),
            new(OpCodes.Ret)
        };
    }
}

[HarmonyPatch(typeof(TextAsset))]
public static class TextAssetPatches {

    [HarmonyTranspiler]
    [HarmonyPatch("Internal_CreateInstance")]
    static IEnumerable<CodeInstruction> Internal_CreateInstance(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.CreateTextAsset)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("get_text")]
    static IEnumerable<CodeInstruction> get_text(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetTextAssetText)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("get_bytes")]
    static IEnumerable<CodeInstruction> get_bytes(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetTextAssetBytes)),
            new(OpCodes.Ret)
        };
    }
}

[HarmonyPatch(typeof(UnityEngine.Random))]
public static class RandomPatches {

    private static readonly System.Random Rng = new();

    [HarmonyTranspiler]
    [HarmonyPatch("get_value")]
    static IEnumerable<CodeInstruction> get_value(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            CodeInstruction.Call(typeof(RandomPatches), nameof(GetValue)),
            new(OpCodes.Ret)
        };
    }

    public static float GetValue() => (float) Rng.NextDouble();

    [HarmonyTranspiler]
    [HarmonyPatch("Range", typeof(int), typeof(int))]
    static IEnumerable<CodeInstruction> Range_Int(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            CodeInstruction.Call(typeof(RandomPatches), nameof(RangeInt)),
            new(OpCodes.Ret)
        };
    }

    public static int RangeInt(int min, int max) => Rng.Next(min, max);

    [HarmonyTranspiler]
    [HarmonyPatch("Range", typeof(float), typeof(float))]
    static IEnumerable<CodeInstruction> Range_Float(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            CodeInstruction.Call(typeof(RandomPatches), nameof(RangeFloat)),
            new(OpCodes.Ret)
        };
    }

    public static float RangeFloat(float min, float max) => min + (float) Rng.NextDouble() * (max - min);

    [HarmonyTranspiler]
    [HarmonyPatch("set_seed")]
    static IEnumerable<CodeInstruction> set_seed(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> { new(OpCodes.Ret) };
    }
}

[HarmonyPatch(typeof(UnityEngine.Debug))]
public static class DebugPatches {

    [HarmonyPrefix]
    [HarmonyPatch("get_unityLogger")]
    static bool get_unityLogger(ref ILogger __result) {
        __result = null;
        return false;
    }

    // Redirect Debug.LogError → Console.Error so KMonoBehaviour.Spawn()'s internal try/catch
    // (which calls DebugUtil.LogExceptionCallstack → Debug.LogError) is visible in headless output.
    // Without this, ALL OnSpawn() failures are completely silent — root cause of invisible bugs.
    [HarmonyPrefix]
    [HarmonyPatch("LogError", typeof(object))]
    static bool LogError(object message) {
        Console.Error.WriteLine($"[Unity.LogError] {message}");
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch("LogError", typeof(object), typeof(UnityEngine.Object))]
    static bool LogErrorWithContext(object message) {
        Console.Error.WriteLine($"[Unity.LogError] {message}");
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch("LogWarning", typeof(object))]
    static bool LogWarning(object message) {
        Console.WriteLine($"[Unity.LogWarn] {message}");
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch("LogWarning", typeof(object), typeof(UnityEngine.Object))]
    static bool LogWarningWithContext(object message) {
        Console.WriteLine($"[Unity.LogWarn] {message}");
        return false;
    }
}


[HarmonyPatch(typeof(SystemInfo))]
public static class SystemInfoPatches {

    [HarmonyTranspiler]
    [HarmonyPatch("get_processorCount")]
    static IEnumerable<CodeInstruction> get_processorCount(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetProcessorCount)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("get_systemMemorySize")]
    static IEnumerable<CodeInstruction> get_systemMemorySize(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetSystemMemorySize)),
            new(OpCodes.Ret)
        };
    }
}

