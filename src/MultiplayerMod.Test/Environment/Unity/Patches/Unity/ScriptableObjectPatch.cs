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

    /// <summary>
    /// Callback invoked after ScriptableObject creation.
    /// Set this from DedicatedServer to inject TextAsset fields (modifiersFile, etc.)
    /// that Unity normally deserializes from scene/prefab data.
    /// </summary>
    public static Action<ScriptableObject>? OnCreated;

    public static ScriptableObject CreateScriptableObjectInstanceFromType(Type type) {
        var obj = (ScriptableObject) Activator.CreateInstance(type);
        OnCreated?.Invoke(obj);
        return obj;
    }
}
