using HarmonyLib;
using MultiplayerMod.multiplayer;
using MultiplayerMod.steam;
using Steamworks;
using UnityEngine;

namespace MultiplayerMod.patch
{

    [HarmonyPatch(typeof(MainMenu), "OnPrefabInit")]
    public class MainMenuPatch
    {
        public static void Prefix(MainMenu __instance)
        {
            MultiplayerState.MainMenu();

            if (Object.FindObjectOfType<Client>() == null)
            {
                var multiplayerGameObject = new GameObject();
                multiplayerGameObject.AddComponent<Client>();
                multiplayerGameObject.AddComponent<ClientActions>();
                Object.DontDestroyOnLoad(multiplayerGameObject);
            }

            __instance.AddButton(
                "Load MultiPlayer",
                true,
                () =>
                {
                    MultiplayerState.SetRoleToHost();
                    __instance.InvokePrivate("LoadGame");
                }
            );
            __instance.AddButton(
                "Start MultiPlayer",
                false,
                () =>
                {
                    MultiplayerState.SetRoleToHost();
                    __instance.InvokePrivate("NewGame");
                }
            );
            __instance.AddButton(
                "Join MultiPlayer",
                false,
                () => { SteamFriends.ActivateGameOverlay("friends"); }
            );
        }
    }

    internal static class Extension
    {
        public static void AddButton(this MainMenu mainMenu, string text, bool topStyle, System.Action action)
        {
            var buttonInfo = typeof(MainMenu).CreateNestedTypeInstance<object>(
                "ButtonInfo",
                new LocString(text),
                action,
                22,
                mainMenu.GetPrivateField<ColorStyleSetting>(topStyle ? "topButtonStyle" : "normalButtonStyle")
            );
            mainMenu.InvokePrivate("MakeButton", buttonInfo);
        }
    }

}
