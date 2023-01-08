using System;
using System.Reflection;
using HarmonyLib;
using MultiplayerMod.steam;

namespace MultiplayerMod.patch
{
    [HarmonyPatch(typeof(MainMenu))]
    [HarmonyPatch("OnPrefabInit")]
    public class MainMenuPatch
    {
        public static void Prefix(MainMenu __instance)
        {
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
            __instance.AddButton("Join MultiPlayer", false, Client.JoinToFriend);
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
                            .GetValue(mainMenu))
                });
        }
    }
}