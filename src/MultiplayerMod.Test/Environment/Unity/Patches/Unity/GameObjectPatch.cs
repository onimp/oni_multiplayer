using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MultiplayerMod.Test.Environment.Unity.Patches.Unity;

[UsedImplicitly]
[HarmonyPatch(typeof(GameObject))]
public class GameObjectPatch {

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("Internal_CreateGameObject")]
    private static IEnumerable<CodeInstruction> GameObject_Internal_CreateGameObject(
        [UsedImplicitly] IEnumerable<CodeInstruction> instructions
    ) {
        var result = new List<CodeInstruction> {
            new(OpCodes.Ldarg_0), // self
            CodeInstruction.Call(typeof(UnityTestRuntime), nameof(UnityTestRuntime.RegisterGameObject)),
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
    [HarmonyPatch("GetComponentFastPath")]
    private static IEnumerable<CodeInstruction> GameObject_GetComponentFastPath(
        IEnumerable<CodeInstruction> instructions
    ) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0), // this
            new(OpCodes.Ldarg_1), // typeof(T)
            new(OpCodes.Ldarg_2), // oneFurtherThanResultValue
            CodeInstruction.Call(typeof(UnityTestRuntime), nameof(UnityTestRuntime.GetComponentFastPath)),
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("GetComponent", new[] { typeof(Type) })]
    private static IEnumerable<CodeInstruction> GameObject_GetComponent(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0), // this
            new(OpCodes.Ldarg_1), // type
            CodeInstruction.Call(typeof(UnityTestRuntime), nameof(UnityTestRuntime.GetComponent)),
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("GetComponentsInternal")]
    private static IEnumerable<CodeInstruction> GameObject_GetComponentsInternal(
        IEnumerable<CodeInstruction> instructions
    ) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0), // this
            new(OpCodes.Ldarg_1), // type
            CodeInstruction.Call(typeof(UnityTestRuntime), nameof(UnityTestRuntime.GetComponentsInternal)),
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("GetComponentInChildren", typeof(Type), typeof(bool))]
    private static IEnumerable<CodeInstruction> GameObject_GetComponentInChildren(
        IEnumerable<CodeInstruction> instructions
    ) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0), // this
            new(OpCodes.Ldarg_1), // type
            new(OpCodes.Ldarg_2), // includeInactive
            CodeInstruction.Call(typeof(UnityTestRuntime), nameof(UnityTestRuntime.GetComponentInChildren)),
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("get_transform")]
    private static IEnumerable<CodeInstruction> GameObject_get_transform(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0), // this
            CodeInstruction.Call(typeof(GameObjectPatch), nameof(GetTransform)),
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("SetActive")]
    private static IEnumerable<CodeInstruction> GameObject_SetActive(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
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

    public static Transform GetTransform(GameObject gameObject) {
        return gameObject.GetComponent<Transform>();
    }

}
