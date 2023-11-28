using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;

namespace MultiplayerMod.Test.Environment.Unity.Patches.Unity;

[UsedImplicitly]
[HarmonyPatch(typeof(UnityEngine.ResourcesAPIInternal))]
public class ResourcesAPIInternalPatch {

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("Load")]
    private static IEnumerable<CodeInstruction> ResourcesAPIInternal_Load(
        IEnumerable<CodeInstruction> instructions
    ) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_1), // type
            CodeInstruction.Call(typeof(ResourcesAPIInternalPatch), nameof(Load)),
            new(OpCodes.Ret)
        };
    }

    public static object Load(Type type) {
        return Activator.CreateInstance(type);
    }
}
