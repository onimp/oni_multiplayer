using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using Object = System.Object;

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

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("FindShaderByName")]
    private static IEnumerable<CodeInstruction> ResourcesAPIInternal_FindShaderByName(
        IEnumerable<CodeInstruction> instructions
    ) {
        return new List<CodeInstruction> {
            CodeInstruction.Call(typeof(ResourcesAPIInternalPatch), nameof(FindShaderByName)),
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("FindObjectsOfTypeAll")]
    private static IEnumerable<CodeInstruction> ResourcesAPIInternal_FindObjectsOfTypeAll(
        IEnumerable<CodeInstruction> instructions
    ) {
        return new List<CodeInstruction> {
            CodeInstruction.Call(typeof(ResourcesAPIInternalPatch), nameof(FindObjectsOfTypeAll)),
            new(OpCodes.Ret)
        };
    }

    public static object Load(Type type) {
        return Activator.CreateInstance(type);
    }

    public static Shader FindShaderByName() {
        return null!;
    }

    public static Object[] FindObjectsOfTypeAll() {
        return Array.Empty<object>();
    }
}
