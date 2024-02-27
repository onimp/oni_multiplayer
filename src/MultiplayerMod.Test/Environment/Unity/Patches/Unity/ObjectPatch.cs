using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MultiplayerMod.Test.Environment.Unity.Patches.Unity;

[UsedImplicitly]
[HarmonyPatch(typeof(Object))]
public class ObjectPatch {

    [UsedImplicitly]
    [HarmonyPostfix]
    [HarmonyPatch(MethodType.Constructor)]
    private static void Object_Constructor(Object __instance) {
        UnityPlayerObjectManager.Allocate(__instance);
        if (__instance is not GameObject) {
            UnityTestRuntime.RegisterObject(__instance, null);
        }
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("DontDestroyOnLoad")]
    private static IEnumerable<CodeInstruction> Object_DontDestroyOnLoad(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("Destroy", typeof(Object), typeof(float))]
    private static IEnumerable<CodeInstruction> Object_Destroy(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            CodeInstruction.Call(typeof(UnityTestRuntime), nameof(UnityTestRuntime.Destroy)),
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("Internal_InstantiateSingle_Injected")]
    private static IEnumerable<CodeInstruction> Object_Internal_InstantiateSingle_Injected(
        IEnumerable<CodeInstruction> instructions
    ) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            CodeInstruction.Call(typeof(UnityTestRuntime), nameof(UnityTestRuntime.Clone)),
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("Internal_CloneSingle")]
    private static IEnumerable<CodeInstruction> Object_Internal_CloneSingle(
        IEnumerable<CodeInstruction> instructions
    ) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            CodeInstruction.Call(typeof(UnityTestRuntime), nameof(UnityTestRuntime.Clone)),
            new(OpCodes.Ret)
        };
    }

    // Just for consistency the value is based on the UnityPlayer!Object structure.
    public static readonly int OffsetOfInstanceIDInCPlusPlusObject = 0x08;

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("GetOffsetOfInstanceIDInCPlusPlusObject")]
    private static IEnumerable<CodeInstruction> Object_GetOffsetOfInstanceIDInCPlusPlusObject(
        IEnumerable<CodeInstruction> instructions
    ) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldc_I4, OffsetOfInstanceIDInCPlusPlusObject),
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Object.ToString), new Type[] { })]
    private static bool Object_ToString(Object __instance, ref string __result) {
        __result = __instance.name + __instance.GetHashCode();
        return false;
    }

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch("get_name")]
    private static bool Object_get_name(Object __instance, ref string __result) {
        __result = UnityTestRuntime.GetName(__instance);
        return false;
    }

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch("set_name")]
    private static bool Object_set_name(Object __instance, string value) {
        UnityTestRuntime.SetName(__instance, value);
        return false;
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("FindObjectsOfType", typeof(Type), typeof(bool))]
    private static IEnumerable<CodeInstruction> Object_FindObjectsOfType(
        IEnumerable<CodeInstruction> instructions
    ) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0), // type
            new(OpCodes.Ldarg_1), // includeInactive
            CodeInstruction.Call(typeof(UnityTestRuntime), nameof(UnityTestRuntime.FindObjectsOfType)),
            new(OpCodes.Ret)
        };
    }

}
