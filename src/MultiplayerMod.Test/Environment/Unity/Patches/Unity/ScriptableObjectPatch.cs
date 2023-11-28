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
        return false;
    }
}
