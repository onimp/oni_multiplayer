using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Multiplayer.UI.Overlays;

namespace MultiplayerMod.Test.Multiplayer;

[UsedImplicitly]
[HarmonyPatch(typeof(MultiplayerStatusOverlay))]
public class MultiplayerStatusOverlayPatch {

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch(nameof(MultiplayerStatusOverlay.Show))]
    private static bool Show() {
        return false;
    }

}
