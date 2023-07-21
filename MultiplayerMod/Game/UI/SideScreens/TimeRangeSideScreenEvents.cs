using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.UI.SideScreens;

[HarmonyPatch(typeof(TimeRangeSideScreen))]
public static class TimeRangeSideScreenEvents {

    public static event Action<ComponentReference, TimeRangeSideScreenEventArgs>? UpdateLogicTimeOfDaySensor;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(TimeRangeSideScreen.ChangeSetting))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void ChangeSetting(TimeRangeSideScreen __instance) =>
        PatchControl.RunIfEnabled(
            () => UpdateLogicTimeOfDaySensor?.Invoke(
                __instance.targetTimedSwitch.GetReference(),
                new TimeRangeSideScreenEventArgs(
                    __instance.targetTimedSwitch.startTime,
                    __instance.targetTimedSwitch.duration
                )
            )
        );

    [Serializable]
    public record TimeRangeSideScreenEventArgs(
        float StartTime,
        float Duration
    );
}
