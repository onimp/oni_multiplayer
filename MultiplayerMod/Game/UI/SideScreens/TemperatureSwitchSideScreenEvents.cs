using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.UI.SideScreens;

[HarmonyPatch(typeof(TemperatureSwitchSideScreen))]
public static class TemperatureSwitchSideScreenEvents {

    public static event Action<ComponentReference, TemperatureSwitchSideScreenEventArgs>? UpdateTemperatureSwitch;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(TemperatureSwitchSideScreen.OnTargetTemperatureChanged))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void OnTargetTemperatureChanged(TemperatureSwitchSideScreen __instance) =>
        PatchControl.RunIfEnabled(
            () => UpdateTemperatureSwitch?.Invoke(
                __instance.targetTemperatureSwitch.GetGridReference(),
                new TemperatureSwitchSideScreenEventArgs(
                    __instance.targetTemperatureSwitch.thresholdTemperature,
                    __instance.targetTemperatureSwitch.activateOnWarmerThan
                )
            )
        );

    [HarmonyPostfix]
    [HarmonyPatch(nameof(TemperatureSwitchSideScreen.OnConditionButtonClicked))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void OnConditionButtonClicked(TemperatureSwitchSideScreen __instance) =>
        PatchControl.RunIfEnabled(
            () => UpdateTemperatureSwitch?.Invoke(
                __instance.targetTemperatureSwitch.GetGridReference(),
                new TemperatureSwitchSideScreenEventArgs(
                    __instance.targetTemperatureSwitch.thresholdTemperature,
                    __instance.targetTemperatureSwitch.activateOnWarmerThan
                )
            )
        );

    [Serializable]
    public record TemperatureSwitchSideScreenEventArgs(
        float ThresholdTemperature,
        bool ActivateOnWarmerThan
    );
}
