using HarmonyLib;

namespace MultiplayerMod.Game.UI;

[HarmonyPatch(typeof(MainMenu))]
// ReSharper disable once UnusedType.Global
public static class MainMenuEvents {

    public static event System.Action? Initialized;

    // ReSharper disable once UnusedMember.Local
    [HarmonyPostfix]
    [HarmonyPatch(nameof(MainMenu.OnSpawn))]
    private static void AfterSpawn() => Initialized?.Invoke();

}
