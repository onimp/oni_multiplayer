using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Game;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer.CoreOperations.Events;

namespace MultiplayerMod.Multiplayer.Patches;

[UsedImplicitly]
[HarmonyPatch(typeof(MainMenu))]
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
        info.action = () => DisableMultiplayer(originalAction);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(MainMenu.ResumeGame))]
    [UsedImplicitly]
    private static void ResumeGame() => DisableMultiplayer();

    private static bool Is(this MainMenu.ButtonInfo info, LocString loc) => info.text.key.String == loc.key.String;

    private static void DisableMultiplayer(System.Action? action = null) {
        Dependencies.Get<EventDispatcher>().Dispatch(new SinglePlayerModeSelectedEvent());
        action?.Invoke();
    }

    private static void UseMultiplayerMode(MultiplayerMode mode, System.Action action) {
        Dependencies.Get<MultiplayerGame>().Refresh(mode);
        Dependencies.Get<EventDispatcher>().Dispatch(new MultiplayerModeSelectedEvent(mode));
        action();
    }

}
