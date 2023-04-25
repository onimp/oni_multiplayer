using HarmonyLib;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Game.World;

[HarmonyPatch(typeof(SaveLoader))]
public abstract class SaveLoaderEvents {

    public static event System.Action WorldSaved;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(SaveLoader.Save), typeof(string), typeof(bool), typeof(bool))]
    public static void SavePostfix() => PatchControl.RunIfEnabled(() => { WorldSaved?.Invoke(); });

}
