using System;
using System.Reflection;
using HarmonyLib;
using MultiplayerMod.multiplayer;
using MultiplayerMod.steam;
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
            multiplayerGameObject.AddComponent<Server>();
            multiplayerGameObject.AddComponent<Client>();
            multiplayerGameObject.AddComponent<ClientActions>();
            multiplayerGameObject.AddComponent<ServerActions>();
            Object.DontDestroyOnLoad(multiplayerGameObject);
            __instance.AddButton("Load MultiPlayer", true,
                () =>
                {
                    var methodInfo =
                        typeof(MainMenu).GetMethod("LoadGame", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (methodInfo == null)
                    {
                        Debug.Log("methodInfo not found");
                        return;
                    }


                    Server.HostServerAfterInit();

                    methodInfo.Invoke(__instance, new object[] { });
                });
            __instance.AddButton("Start MultiPlayer", false,
                () =>
                {
                    var methodInfo =
                        typeof(MainMenu).GetMethod("NewGame", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (methodInfo == null)
                    {
                        Debug.Log("methodInfo not found");
                        return;
                    }

                    Server.HostServerAfterInit();

                    methodInfo.Invoke(__instance, new object[] { });
                });
            __instance.AddButton("Join MultiPlayer", false, Client.ShowJoinToFriend);
        }
    }

    static class Extension
    {
        public static void AddButton(this MainMenu mainMenu, string text, bool topStyle, System.Action action)
        {
            var type = typeof(MainMenu);
            var makeButtonMethodInfo = type.GetMethod("MakeButton", BindingFlags.NonPublic | BindingFlags.Instance);
            if (makeButtonMethodInfo == null)
            {
                Debug.Log("Button not found");
                return;
            }

            var buttonInfoType = type.GetNestedType("ButtonInfo", BindingFlags.NonPublic | BindingFlags.Instance);

            makeButtonMethodInfo.Invoke(
                mainMenu,
                new[]
                {
                    Activator.CreateInstance(
                        buttonInfoType,
                        new LocString(text),
                        action,
                        22,
                        type.GetField(topStyle ? "topButtonStyle" : "normalButtonStyle",
                                BindingFlags.NonPublic | BindingFlags.Instance)
                            ?.GetValue(mainMenu))
                });
        }
    }
}