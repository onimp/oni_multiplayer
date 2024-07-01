using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.ModRuntime.Context;

namespace MultiplayerMod.Game.World;

[HarmonyPatch(typeof(SaveLoader))]
public static class SaveLoaderEvents {

    public static event System.Action? WorldSaved;
    public static event System.Action? WorldLoading;

    [HarmonyPostfix, UsedImplicitly]
    [HarmonyPatch(nameof(SaveLoader.Save), typeof(string), typeof(bool), typeof(bool))]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    public static void SavePostfix() => WorldSaved?.Invoke();

    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch(nameof(SaveLoader.Load), typeof(string))]
    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    public static void LoadPrefix() => WorldLoading?.Invoke();

}
