using HarmonyLib;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Game;

[HarmonyPatch(typeof(SpeedControlScreen))]
public static class GameSpeedControlEvents {

    public static event System.Action GamePaused;
    public static event System.Action GameResumed;
    public static event SpeedChangedEventHandler SpeedChanged;

    // ReSharper disable once InconsistentNaming
    [HarmonyPostfix]
    [HarmonyPatch(nameof(SpeedControlScreen.SetSpeed))]
    private static void SetSpeedPostfix(int Speed) => PatchControl.RunIfEnabled(() => { SpeedChanged?.Invoke(Speed); });

    [HarmonyPostfix]
    [HarmonyPatch(nameof(SpeedControlScreen.OnPause))]
    private static void OnPausePostfix() => PatchControl.RunIfEnabled(() => GamePaused?.Invoke());

    [HarmonyPostfix]
    [HarmonyPatch(nameof(SpeedControlScreen.OnPlay))]
    private static void OnPlayPostfix() => PatchControl.RunIfEnabled(() => GameResumed?.Invoke());

}

public delegate void SpeedChangedEventHandler(int speed);
