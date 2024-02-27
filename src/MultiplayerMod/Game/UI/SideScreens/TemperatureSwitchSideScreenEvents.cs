using System;
using HarmonyLib;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.UI.SideScreens;

[HarmonyPatch(typeof(TemperatureSwitchSideScreen))]
public static class TemperatureSwitchSideScreenEvents {

    public static event Action<TemperatureSwitchSideScreenEventArgs>? UpdateTemperatureSwitch;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(TemperatureSwitchSideScreen.OnTargetTemperatureChanged))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void OnTargetTemperatureChanged(TemperatureSwitchSideScreen __instance) => TriggerEvent(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(nameof(TemperatureSwitchSideScreen.OnConditionButtonClicked))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void OnConditionButtonClicked(TemperatureSwitchSideScreen __instance) => TriggerEvent(__instance);

    [RequireExecutionLevel(ExecutionLevel.Game)]
    private static void TriggerEvent(TemperatureSwitchSideScreen instance) => UpdateTemperatureSwitch?.Invoke(
        new TemperatureSwitchSideScreenEventArgs(
            instance.targetTemperatureSwitch.GetReference(),
            instance.targetTemperatureSwitch.thresholdTemperature,
            instance.targetTemperatureSwitch.activateOnWarmerThan
        )
    );

    [Serializable]
    public record TemperatureSwitchSideScreenEventArgs(
        ComponentReference<TemperatureControlledSwitch> Target,
        float ThresholdTemperature,
        bool ActivateOnWarmerThan
    );

}
