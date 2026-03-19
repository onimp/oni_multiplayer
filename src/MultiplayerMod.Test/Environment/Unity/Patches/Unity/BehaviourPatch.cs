using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MultiplayerMod.Test.Environment.Unity.Patches.Unity;

[UsedImplicitly]
[HarmonyPatch(typeof(Behaviour))]
public class BehaviourPatch {

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("get_isActiveAndEnabled")]
    private static IEnumerable<CodeInstruction> Behaviour_get_isActiveAndEnabled(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldc_I4_1), // true
            new(OpCodes.Ret)
        };
    }


}
