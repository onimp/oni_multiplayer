using HarmonyLib;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Game.World;

[HarmonyPatch(typeof(SaveLoader))]
public static class SaveLoaderEvents {

    public static event System.Action? WorldSaved;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(SaveLoader.Save), typeof(string), typeof(bool), typeof(bool))]
    // ReSharper disable once UnusedMember.Global
    public static void SavePostfix() => PatchControl.RunIfEnabled(() => { WorldSaved?.Invoke(); });

}
