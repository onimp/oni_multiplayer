using HarmonyLib;

namespace MultiplayerMod.Game.UI.Screens.Events;

[HarmonyPatch(typeof(PauseScreen))]
public static class PauseScreenEvents {

    public static event System.Action? QuitGame;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(PauseScreen.TriggerQuitGame))]
    // ReSharper disable once UnusedMember.Local
    private static void TriggerQuitGame() => QuitGame?.Invoke();

}
