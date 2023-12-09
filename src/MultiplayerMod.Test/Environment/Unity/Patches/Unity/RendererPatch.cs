using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MultiplayerMod.Test.Environment.Unity.Patches.Unity;

[UsedImplicitly]
[HarmonyPatch(typeof(Renderer))]
public class RendererPatch {

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("get_material")]
    private static IEnumerable<CodeInstruction> Renderer_get_material(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0), // this
            CodeInstruction.Call(typeof(RendererPatch), nameof(CreateMaterial)),
            new(OpCodes.Ret)
        };
    }

    public static Material CreateMaterial(Renderer _) {
#pragma warning disable CS0618 // Type or member is obsolete
        return new Material("");
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
