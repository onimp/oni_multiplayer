using HarmonyLib;
using JetBrains.Annotations;

namespace MultiplayerMod.Test.Environment.Unity.Patches;

[UsedImplicitly]
[HarmonyPatch(typeof(LoadingOverlay))]
public static class LoadingOverlayPatch {

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch(nameof(LoadingOverlay.Load))]
    private static bool Load(System.Action cb) {
        cb();
        return false;
    }

}
