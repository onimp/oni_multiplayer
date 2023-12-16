using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MultiplayerMod.Test.Environment.Unity.Patches.Unity;

[UsedImplicitly]
[HarmonyPatch(typeof(Shader))]
public class ShaderPatch {

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Shader.PropertyToID))]
    private static IEnumerable<CodeInstruction> SystemInfo_get_processorCount(
        IEnumerable<CodeInstruction> instructions
    ) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldc_I4_0), // 0
            new(OpCodes.Ret)
        };
    }
}
