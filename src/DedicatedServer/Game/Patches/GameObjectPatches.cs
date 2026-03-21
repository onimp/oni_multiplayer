using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace DedicatedServer.Game.Patches;

/// <summary>
/// Patches for UnityEngine.GameObject — create, find, components, active, transform.
/// </summary>
[HarmonyPatch(typeof(GameObject))]
public static class GameObjectPatches {

    [HarmonyTranspiler]
    [HarmonyPatch("Internal_CreateGameObject")]
    static IEnumerable<CodeInstruction> GameObject_Internal_CreateGameObject(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0), // self
            new(OpCodes.Ldarg_1), // name
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.CreateGameObject)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("Find")]
    static IEnumerable<CodeInstruction> GameObject_Find(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.Find)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("GetComponent", new[] { typeof(Type) })]
    static IEnumerable<CodeInstruction> GameObject_GetComponent(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetComponent)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("GetComponentFastPath")]
    static IEnumerable<CodeInstruction> GameObject_GetComponentFastPath(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            new(OpCodes.Ldarg_2),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetComponentFastPath)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("GetComponentsInternal")]
    static IEnumerable<CodeInstruction> GameObject_GetComponentsInternal(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetComponentsInternal)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("GetComponentInChildren", typeof(Type), typeof(bool))]
    static IEnumerable<CodeInstruction> GameObject_GetComponentInChildren(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            new(OpCodes.Ldarg_2),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetComponentInChildren)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("Internal_AddComponentWithType")]
    static IEnumerable<CodeInstruction> GameObject_Internal_AddComponentWithType(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.AddComponent)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("SetActive")]
    static IEnumerable<CodeInstruction> GameObject_SetActive(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.SetActive)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("get_activeSelf")]
    static IEnumerable<CodeInstruction> GameObject_get_activeSelf(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetActive)),
            new(OpCodes.Ret)
        };
    }

    // activeInHierarchy: in headless there is no parent hierarchy — same as activeSelf
    [HarmonyTranspiler]
    [HarmonyPatch("get_activeInHierarchy")]
    static IEnumerable<CodeInstruction> GameObject_get_activeInHierarchy(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetActive)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("get_transform")]
    static IEnumerable<CodeInstruction> GameObject_get_transform(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetTransformFromGameObject)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("set_layer")]
    static IEnumerable<CodeInstruction> GameObject_set_layer(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> { new(OpCodes.Ret) };
    }
}
