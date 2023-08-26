using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MultiplayerMod.Test.Environment.Unity.Patches;

[UsedImplicitly]
[HarmonyPatch(typeof(Object))]
public class UnityObjectPatch {

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Object), "DontDestroyOnLoad")]
    private static IEnumerable<CodeInstruction> Object_DontDestroyOnLoad(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ret)
        };
    }

}
