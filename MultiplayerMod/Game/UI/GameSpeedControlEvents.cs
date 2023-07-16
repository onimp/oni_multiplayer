using HarmonyLib;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Game.UI;

[HarmonyPatch(typeof(SpeedControlScreen))]
public static class GameSpeedControlEvents {

    public static event System.Action? GamePaused;
    public static event System.Action? GameResumed;
    public static event SpeedChangedEventHandler? SpeedChanged;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(SpeedControlScreen.SetSpeed))]
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Local
    private static void SetSpeedPostfix(int Speed) => PatchControl.RunIfEnabled(() => { SpeedChanged?.Invoke(Speed); });

    [HarmonyPostfix]
    [HarmonyPatch(nameof(SpeedControlScreen.OnPause))]
    // ReSharper disable once UnusedMember.Local
    private static void OnPausePostfix() => PatchControl.RunIfEnabled(() => GamePaused?.Invoke());

    [HarmonyPostfix]
    [HarmonyPatch(nameof(SpeedControlScreen.OnPlay))]
    // ReSharper disable once UnusedMember.Local
    private static void OnPlayPostfix() => PatchControl.RunIfEnabled(() => GameResumed?.Invoke());

}

public delegate void SpeedChangedEventHandler(int speed);
