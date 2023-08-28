using HarmonyLib;
using MultiplayerMod.ModRuntime.Context;

namespace MultiplayerMod.Game.World;

[HarmonyPatch(typeof(SaveLoader))]
public static class SaveLoaderEvents {

    public static event System.Action? WorldSaved;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(SaveLoader.Save), typeof(string), typeof(bool), typeof(bool))]
    [RequireExecutionLevel(ExecutionLevel.Gameplay)]
    // ReSharper disable once UnusedMember.Global
    public static void SavePostfix() => WorldSaved?.Invoke();

}
