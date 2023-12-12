using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Unity;
using UnityEngine;

namespace MultiplayerMod.Test.Environment.Unity.Patches.Unity;

[UsedImplicitly]
[HarmonyPatch(typeof(ScriptableObject))]
public class ScriptableObjectPatch {

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch(MethodType.Constructor)]
    private static bool ScriptableObject_Constructor(ScriptableObject __instance) {
        UnityObject.MarkAsNotNull(__instance);
        UnityTestRuntime.SetName(__instance, $"New {__instance.GetType()}");
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
