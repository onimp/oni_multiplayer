using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DedicatedServer.Game.Patches;

/// <summary>
/// Patches for UnityEngine.Object — constructor, name, destroy, instantiate, find.
/// </summary>
[HarmonyPatch(typeof(Object))]
public static class ObjectPatches {

    [HarmonyPostfix]
    [HarmonyPatch(MethodType.Constructor)]
    static void Object_Constructor(Object __instance) {
        UnityRuntime.ObjectConstructor(__instance);
    }

    [HarmonyTranspiler]
    [HarmonyPatch("get_name")]
    static IEnumerable<CodeInstruction> Object_get_name(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetName)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("set_name")]
    static IEnumerable<CodeInstruction> Object_set_name(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.SetName)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Object.ToString), new Type[] { })]
    static IEnumerable<CodeInstruction> Object_ToString(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.ObjectToString)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("DontDestroyOnLoad")]
    static IEnumerable<CodeInstruction> Object_DontDestroyOnLoad(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> { new(OpCodes.Ret) };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("Destroy", typeof(Object), typeof(float))]
    static IEnumerable<CodeInstruction> Object_Destroy(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldc_R4, 0f),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.DestroyObject)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("DestroyImmediate", typeof(Object), typeof(bool))]
    static IEnumerable<CodeInstruction> Object_DestroyImmediate(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldc_I4_0),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.DestroyImmediate)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("Internal_CloneSingle")]
    static IEnumerable<CodeInstruction> Object_Internal_CloneSingle(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.CloneSingle)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("Internal_InstantiateSingle_Injected")]
    static IEnumerable<CodeInstruction> Object_Internal_InstantiateSingle_Injected(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.CloneSingle)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("GetOffsetOfInstanceIDInCPlusPlusObject")]
    static IEnumerable<CodeInstruction> Object_GetOffsetOfInstanceIDInCPlusPlusObject(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldc_I4, 0x08),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("FindObjectsOfType", typeof(Type), typeof(bool))]
    static IEnumerable<CodeInstruction> Object_FindObjectsOfType(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.FindObjectsOfType)),
            new(OpCodes.Ret)
        };
    }
}
