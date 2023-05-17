using HarmonyLib;

namespace MultiplayerMod.Game.World;

[HarmonyPatch(typeof(PauseScreen))]
public static class PauseScreenEvents {

    public static event System.Action OnDestroy;

    [HarmonyPostfix]
    [HarmonyPatch("TriggerQuitGame")]
    private static void TriggerQuitGame() => OnDestroy?.Invoke();

}
