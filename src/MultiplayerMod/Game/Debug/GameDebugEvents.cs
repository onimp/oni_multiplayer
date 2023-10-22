using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.ModRuntime.Context;

namespace MultiplayerMod.Game.Debug;

[HarmonyPatch]
public static class GameDebugEvents {

    public static event System.Action? GameFrameStepping;
    public static event System.Action? SimulationStepping;

    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch(typeof(SpeedControlScreen), nameof(SpeedControlScreen.DebugStepFrame))]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    private static void DebugStepFramePrefix() => GameFrameStepping?.Invoke();

    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch(typeof(global::Game), nameof(global::Game.ForceSimStep))]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    private static void ForceSimStepPrefix() => SimulationStepping?.Invoke();

}
