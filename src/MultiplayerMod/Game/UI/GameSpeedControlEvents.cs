using System;
using HarmonyLib;
using MultiplayerMod.ModRuntime.Context;

namespace MultiplayerMod.Game.UI;

[HarmonyPatch(typeof(SpeedControlScreen))]
public static class GameSpeedControlEvents {

    public static event System.Action? GamePaused;
    public static event System.Action? GameResumed;
    public static event Action<int>? SpeedChanged;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(SpeedControlScreen.SetSpeed))]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Local
    private static void SetSpeedPostfix(int Speed) => SpeedChanged?.Invoke(Speed);

    [HarmonyPostfix]
    [HarmonyPatch(nameof(SpeedControlScreen.OnPause))]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    // ReSharper disable once UnusedMember.Local
    private static void OnPausePostfix() => GamePaused?.Invoke();

    [HarmonyPostfix]
    [HarmonyPatch(nameof(SpeedControlScreen.OnPlay))]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    // ReSharper disable once UnusedMember.Local
    private static void OnPlayPostfix() => GameResumed?.Invoke();

}
