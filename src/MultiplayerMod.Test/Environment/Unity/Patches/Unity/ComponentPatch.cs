using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MultiplayerMod.Test.Environment.Unity.Patches.Unity;

[UsedImplicitly]
[HarmonyPatch(typeof(Component))]
public class ComponentPatch {

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("get_gameObject")]
    private static IEnumerable<CodeInstruction> Component_get_gameObject(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0), // this
            CodeInstruction.Call(typeof(UnityTestRuntime), nameof(UnityTestRuntime.GetGameObject)),
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("get_transform")]
    private static IEnumerable<CodeInstruction> Component_get_transform(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0), // this
            CodeInstruction.Call(typeof(ComponentPatch), nameof(GetTransform)),
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("GetComponentFastPath")]
    private static IEnumerable<CodeInstruction> Component_GetComponentFastPath(
        IEnumerable<CodeInstruction> instructions
    ) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0), // this
            new(OpCodes.Ldarg_1), // typeof(T)
            new(OpCodes.Ldarg_2), // oneFurtherThanResultValue
            CodeInstruction.Call(typeof(UnityTestRuntime), nameof(UnityTestRuntime.GetComponentFastPathFromComponent)),
            new(OpCodes.Ret)
        };
    }

    public static Transform GetTransform(Component component) {
        return component.gameObject.GetComponent<Transform>();
    }
}
