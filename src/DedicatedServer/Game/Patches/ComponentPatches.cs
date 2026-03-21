using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace DedicatedServer.Game.Patches;

/// <summary>
/// Patches for UnityEngine.Component — gameObject, transform, GetComponent.
/// </summary>
[HarmonyPatch(typeof(Component))]
public static class ComponentPatches {

    [HarmonyTranspiler]
    [HarmonyPatch("get_gameObject")]
    static IEnumerable<CodeInstruction> Component_get_gameObject(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetGameObject)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("get_transform")]
    static IEnumerable<CodeInstruction> Component_get_transform(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetTransformFromComponent)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("GetComponentFastPath")]
    static IEnumerable<CodeInstruction> Component_GetComponentFastPath(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            new(OpCodes.Ldarg_2),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetComponentFastPathFromComponent)),
            new(OpCodes.Ret)
        };
    }
}
