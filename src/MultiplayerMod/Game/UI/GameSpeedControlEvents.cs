using System;
using AttributeProcessor.Annotations;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.ModRuntime.Context;

namespace MultiplayerMod.Game.UI;

[HarmonyPatch(typeof(SpeedControlScreen))]
public static class GameSpeedControlEvents {

    public static event System.Action? GamePaused;
    public static event System.Action? GameResumed;
    public static event Action<int>? SpeedChanged;

    private static bool eventsEnabled = true;

    [HarmonyPostfix, UsedImplicitly]
    [HarmonyPatch(nameof(SpeedControlScreen.SetSpeed))]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    [RequireEnabledEvents]
    // ReSharper disable once InconsistentNaming
    private static void SetSpeedPostfix(int Speed) => SpeedChanged?.Invoke(Speed);

    [HarmonyPostfix, UsedImplicitly]
    [HarmonyPatch(nameof(SpeedControlScreen.OnPause))]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    private static void OnPausePostfix() {
        if (!eventsEnabled) {
            eventsEnabled = true;
            return;
        }
        GamePaused?.Invoke();
    }

    [HarmonyPostfix, UsedImplicitly]
    [HarmonyPatch(nameof(SpeedControlScreen.OnPlay))]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    [RequireEnabledEvents]
    private static void OnPlayPostfix() => GameResumed?.Invoke();

    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch(nameof(SpeedControlScreen.DebugStepFrame))]
    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    private static void DebugStepFramePrefix() => eventsEnabled = false;

    [AttributeUsage(AttributeTargets.Method)]
    [ConditionalInvocation(typeof(RequireEnabledEventsAttribute), nameof(CheckEnabled))]
    private class RequireEnabledEventsAttribute : Attribute {

        private static bool CheckEnabled() => eventsEnabled;

    }

}
