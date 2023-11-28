using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Unity;
using Object = UnityEngine.Object;

namespace MultiplayerMod.Test.Environment.Unity.Patches.Unity;

[UsedImplicitly]
[HarmonyPatch(typeof(Object))]
public class ObjectPatch {

    [UsedImplicitly]
    [HarmonyPostfix]
    [HarmonyPatch(MethodType.Constructor)]
    private static void Object_Constructor(Object __instance) {
        UnityObject.MarkAsNotNull(__instance);
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
    [HarmonyPatch("GetOffsetOfInstanceIDInCPlusPlusObject")]
    private static IEnumerable<CodeInstruction> Object_GetOffsetOfInstanceIDInCPlusPlusObject(
        IEnumerable<CodeInstruction> instructions
    ) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldc_I4_0),
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
}
