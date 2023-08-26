using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MultiplayerMod.Test.Environment.Unity.Patches;

[UsedImplicitly]
[HarmonyPatch(typeof(GameObject))]
public class UnityGameObjectPatch {

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("Internal_CreateGameObject")]
    private static IEnumerable<CodeInstruction> GameObject_Internal_CreateGameObject(
        [UsedImplicitly] IEnumerable<CodeInstruction> instructions
    ) {
        var result = new List<CodeInstruction> {
            new(OpCodes.Ldarg_0), // self
            CodeInstruction.Call(typeof(UnityTestRuntime), nameof(UnityTestRuntime.Register)),
            new(OpCodes.Ret)
        };
        return result;
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("Find")]
    private static IEnumerable<CodeInstruction> GameObject_Find(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0), // name
            CodeInstruction.Call(typeof(UnityTestRuntime), nameof(UnityTestRuntime.Find)),
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("Internal_AddComponentWithType")]
    private static IEnumerable<CodeInstruction> GameObject_Internal_AddComponentWithType(
        IEnumerable<CodeInstruction> instructions
    ) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0), // this (go)
            new(OpCodes.Ldarg_1), // componentType
            CodeInstruction.Call(typeof(UnityTestRuntime), nameof(UnityTestRuntime.AddComponent)),
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Time), "get_frameCount")]
    private static IEnumerable<CodeInstruction> Time_get_frameCount(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Call, AccessTools.PropertyGetter(typeof(UnityTestRuntime), nameof(UnityTestRuntime.FrameCount))),
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Component), "get_gameObject")]
    private static IEnumerable<CodeInstruction> Component_get_gameObject(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0), // this (component)
            CodeInstruction.Call(typeof(UnityTestRuntime), nameof(UnityTestRuntime.GetGameObject)),
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Object), "DontDestroyOnLoad")]
    private static IEnumerable<CodeInstruction> Object_DontDestroyOnLoad(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ret)
        };
    }

}
