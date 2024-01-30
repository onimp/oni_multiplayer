using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MultiplayerMod.Test.Environment.Unity.Patches.Unity;

[UsedImplicitly]
[HarmonyPatch(typeof(ScriptableObject))]
public class ScriptableObjectPatch {

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch(MethodType.Constructor)]
    private static bool ScriptableObject_Constructor(ScriptableObject __instance) {
        UnityPlayerObjectManager.Allocate(__instance);
        UnityTestRuntime.RegisterObject(__instance, null);
        return false;
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("CreateScriptableObjectInstanceFromType")]
    private static IEnumerable<CodeInstruction> ScriptableObject_CreateScriptableObjectInstanceFromType(
        IEnumerable<CodeInstruction> instructions
    ) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0), // type
            CodeInstruction.Call(typeof(ScriptableObjectPatch), nameof(CreateScriptableObjectInstanceFromType)),
            new(OpCodes.Ret)
        };
    }

    public static ScriptableObject CreateScriptableObjectInstanceFromType(Type type) {
        return (ScriptableObject) Activator.CreateInstance(type);
    }
}
