using HarmonyLib;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Game;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Multiplayer.Patches;

[HarmonyPatch(typeof(MainMenu))]
// ReSharper disable once UnusedType.Global
internal class MainMenuPatch {

    [HarmonyPrefix]
    [HarmonyPatch("OnPrefabInit")]
    // ReSharper disable once UnusedMember.Local
    private static void OnPrefabInit(MainMenu __instance) {
        Dependencies.Get<MultiplayerGame>().Refresh();
        var operations = Dependencies.Get<IMultiplayerOperations>();
        __instance.AddButton("NEW MULTIPLAYER", highlight: true, CreateHostWrapper(__instance.NewGame));
        __instance.AddButton("LOAD MULTIPLAYER", highlight: false, CreateHostWrapper(__instance.LoadGame));
        __instance.AddButton("JOIN MULTIPLAYER", highlight: false, operations.Join);
    }

    // TODO: Reset multiplayer role to none in case of button action cancellation
    private static System.Action CreateHostWrapper(System.Action action) => () => {
        Dependencies.Get<MultiplayerGame>().Role = MultiplayerRole.Host;
        action();
    };

}
