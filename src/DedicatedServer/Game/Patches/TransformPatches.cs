using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace DedicatedServer.Game.Patches;

/// <summary>
/// Patches for UnityEngine.Transform — position, parent.
/// </summary>
[HarmonyPatch(typeof(Transform))]
public static class TransformPatches {

    [HarmonyTranspiler]
    [HarmonyPatch("get_position_Injected")]
    static IEnumerable<CodeInstruction> get_position_Injected(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetPosition)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("get_localPosition_Injected")]
    static IEnumerable<CodeInstruction> get_localPosition_Injected(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.GetPosition)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("set_position_Injected")]
    static IEnumerable<CodeInstruction> set_position_Injected(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.SetPositionFromTransform)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("set_localPosition_Injected")]
    static IEnumerable<CodeInstruction> set_localPosition_Injected(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            CodeInstruction.Call(typeof(UnityRuntime), nameof(UnityRuntime.SetPositionFromTransform)),
            new(OpCodes.Ret)
        };
    }

    [HarmonyTranspiler]
    [HarmonyPatch("SetParent", typeof(Transform), typeof(bool))]
    static IEnumerable<CodeInstruction> SetParent(IEnumerable<CodeInstruction> _) {
        return new List<CodeInstruction> { new(OpCodes.Ret) };
    }
}
