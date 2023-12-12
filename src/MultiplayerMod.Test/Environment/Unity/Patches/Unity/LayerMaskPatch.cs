using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MultiplayerMod.Test.Environment.Unity.Patches.Unity;

[UsedImplicitly]
[HarmonyPatch(typeof(LayerMask))]
public class LayerMaskPatch {

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("NameToLayer")]
    private static IEnumerable<CodeInstruction> LayerMask_NameToLayer(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldc_I4_0),
            new(OpCodes.Ret)
        };
    }
}
