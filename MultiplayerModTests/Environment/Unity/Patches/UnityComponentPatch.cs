using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MultiplayerModTests.Environment.Unity.Patches;

[UsedImplicitly]
[HarmonyPatch(typeof(Component))]
public class UnityComponentPatch {

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

}
