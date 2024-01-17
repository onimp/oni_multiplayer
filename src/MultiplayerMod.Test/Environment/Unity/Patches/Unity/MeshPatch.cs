using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MultiplayerMod.Test.Environment.Unity.Patches.Unity;

[UsedImplicitly]
[HarmonyPatch(typeof(Mesh))]
public class MeshPatch {

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("Internal_Create")]
    private static IEnumerable<CodeInstruction> Mesh_Internal_Create(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ret)
        };
    }

}
