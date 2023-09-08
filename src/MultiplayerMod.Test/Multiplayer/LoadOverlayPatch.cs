using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Multiplayer.UI;

namespace MultiplayerMod.Test.Multiplayer;

[UsedImplicitly]
[HarmonyPatch(typeof(LoadOverlay))]
public class LoadOverlayPatch {

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch(nameof(LoadOverlay.Show))]
    private static bool Show() {
        return false;
    }

}
