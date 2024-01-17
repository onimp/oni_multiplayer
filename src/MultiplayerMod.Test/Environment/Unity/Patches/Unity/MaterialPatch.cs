using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MultiplayerMod.Test.Environment.Unity.Patches.Unity;

[UsedImplicitly]
[HarmonyPatch(typeof(Material))]
public class MaterialPatch {

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("CreateWithString")]
    private static IEnumerable<CodeInstruction> Material_CreateWithString(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("CreateWithShader")]
    private static IEnumerable<CodeInstruction> Material_CreateWithShader(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("GetFirstPropertyNameIdByAttribute")]
    private static IEnumerable<CodeInstruction> Material_GetFirstPropertyNameIdByAttribute(
        IEnumerable<CodeInstruction> instructions
    ) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldc_I4_0),
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("SetColorImpl")]
    private static IEnumerable<CodeInstruction> Material_SetColorImpl(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ret)
        };
    }
}
