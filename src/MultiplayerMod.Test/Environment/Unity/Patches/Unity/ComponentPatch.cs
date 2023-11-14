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
    [HarmonyPatch(typeof(Component), "get_gameObject")]
    private static IEnumerable<CodeInstruction> Component_get_gameObject(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0), // this
            CodeInstruction.Call(typeof(UnityTestRuntime), nameof(UnityTestRuntime.GetGameObject)),
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
}
