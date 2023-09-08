using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Game;
using MultiplayerMod.ModRuntime.StaticCompatibility;

namespace MultiplayerMod.Multiplayer.Patches;

[HarmonyPatch(typeof(MainMenu))]
// ReSharper disable once UnusedType.Global
internal static class MainMenuPatch {

    [HarmonyPrefix]
    [HarmonyPatch("OnPrefabInit")]
    [UsedImplicitly]
    private static void OnPrefabInit(MainMenu __instance) {
        var operations = Dependencies.Get<IMultiplayerOperations>();
        __instance.AddButton(
            "NEW MULTIPLAYER",
            highlight: true,
            () => UseMultiplayerMode(MultiplayerMode.Host, __instance.NewGame)
        );
        __instance.AddButton(
            "LOAD MULTIPLAYER",
            highlight: false,
            () => UseMultiplayerMode(MultiplayerMode.Host, __instance.LoadGame)
        );
        __instance.AddButton("JOIN MULTIPLAYER", highlight: false, operations.Join);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(MainMenu.MakeButton))]
    [UsedImplicitly]
    private static void MakeButton(ref MainMenu.ButtonInfo info) {
        if (!info.Is(STRINGS.UI.FRONTEND.MAINMENU.NEWGAME) && !info.Is(STRINGS.UI.FRONTEND.MAINMENU.LOADGAME))
            return;

        var originalAction = info.action;
        info.action = () => UseMultiplayerMode(MultiplayerMode.Disabled, originalAction);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(MainMenu.ResumeGame))]
    [UsedImplicitly]
    private static void ResumeGame() => Dependencies.Get<MultiplayerGame>().Refresh(MultiplayerMode.Disabled);

    private static bool Is(this MainMenu.ButtonInfo info, LocString loc) => info.text.key.String == loc.key.String;

    private static void UseMultiplayerMode(MultiplayerMode mode, System.Action action) {
        Dependencies.Get<MultiplayerGame>().Refresh(mode);
        action();
    }

}
