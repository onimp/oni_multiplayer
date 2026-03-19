using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MultiplayerMod.Test.Environment.Unity.Patches.Unity;

[UsedImplicitly]
[HarmonyPatch(typeof(MonoBehaviour))]
public class MonoBehaviourPatch {

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch(nameof(MonoBehaviour.StartCoroutine), typeof(IEnumerator))]
    private static bool StartCoroutine(MonoBehaviour __instance, IEnumerator routine) {
        // Disabled for now, process if required.
        return false;
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("IsObjectMonoBehaviour")]
    private static IEnumerable<CodeInstruction> MonoBehaviour_IsObjectMonoBehaviour(
        IEnumerable<CodeInstruction> instructions
    ) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldc_I4_1), // true — all objects in test env are MonoBehaviours
            new(OpCodes.Ret)
        };
    }

}
