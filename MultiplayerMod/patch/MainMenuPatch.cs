using System;
using System.Reflection;
using HarmonyLib;
using MultiplayerMod.multiplayer;
using MultiplayerMod.steam;
using Steamworks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MultiplayerMod.patch
{
    [HarmonyPatch(typeof(MainMenu), "OnPrefabInit")]
    public class MainMenuPatch
    {
        public static void Prefix(MainMenu __instance)
        {
            var multiplayerGameObject = new GameObject();
            multiplayerGameObject.AddComponent<Client>();
            multiplayerGameObject.AddComponent<ClientActions>();
            // TODO destroy if game is stopped/exit to main menu
            Object.DontDestroyOnLoad(multiplayerGameObject);
            __instance.AddButton("Load MultiPlayer", true,
                () =>
                {
                    SpawnPatch.HostServerAfterStart = true;
                    __instance.InvokePrivate("LoadGame");
                });
            __instance.AddButton("Start MultiPlayer", false,
                () =>
                {
                    SpawnPatch.HostServerAfterStart = true;
                    __instance.InvokePrivate("NewGame");
                });
            __instance.AddButton("Join MultiPlayer", false,
                () => SteamFriends.ActivateGameOverlay("friends"));
        }
    }

    static class Extension
    {
        public static void AddButton(this MainMenu mainMenu, string text, bool topStyle, System.Action action)
        {
            var type = typeof(MainMenu);

            var buttonInfoType = type.GetNestedType("ButtonInfo", BindingFlags.NonPublic | BindingFlags.Instance);

            mainMenu.InvokePrivate("MakeButton",
                Activator.CreateInstance(
                    buttonInfoType,
                    new LocString(text),
                    action,
                    22,
                    mainMenu.GetPrivateField<ColorStyleSetting>(topStyle ? "topButtonStyle" : "normalButtonStyle"))
            );
        }
    }
}