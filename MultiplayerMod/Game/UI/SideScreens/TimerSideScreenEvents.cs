using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.UI.SideScreens;

[HarmonyPatch(typeof(TimerSideScreen))]
public static class TimerSideScreenEvents {

    public static event Action<ComponentReference, TimerSideScreenEventArgs>? UpdateLogicTimeSensor;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(TimerSideScreen.ChangeSetting))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void ChangeSetting(TimerSideScreen __instance) => PatchControl.RunIfEnabled(
        () => UpdateLogicTimeSensor?.Invoke(
            __instance.targetTimedSwitch.GetGridReference(),
            new TimerSideScreenEventArgs(
                __instance.targetTimedSwitch.displayCyclesMode,
                __instance.targetTimedSwitch.onDuration,
                __instance.targetTimedSwitch.offDuration,
                __instance.targetTimedSwitch.timeElapsedInCurrentState
            )
        )
    );

    [HarmonyPostfix]
    [HarmonyPatch(nameof(TimerSideScreen.ToggleMode))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void UpdateMaxCapacity(TimerSideScreen __instance) => PatchControl.RunIfEnabled(
        () => UpdateLogicTimeSensor?.Invoke(
            __instance.targetTimedSwitch.GetGridReference(),
            new TimerSideScreenEventArgs(
                __instance.targetTimedSwitch.displayCyclesMode,
                __instance.targetTimedSwitch.onDuration,
                __instance.targetTimedSwitch.offDuration,
                __instance.targetTimedSwitch.timeElapsedInCurrentState
            )
        )
    );

    [Serializable]
    public record TimerSideScreenEventArgs(
        bool DisplayCyclesMode,
        float OnDuration,
        float OffDuration,
        float TimeElapsedInCurrentState
    );
}
