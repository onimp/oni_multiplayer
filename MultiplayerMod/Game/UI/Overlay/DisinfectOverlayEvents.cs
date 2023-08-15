using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Game.UI.Overlay;

[HarmonyPatch(typeof(DisinfectThresholdDiagram))]
public static class DiseaseOverlayEvents {

    public static event Action<int, bool>? DiseaseSettingsChanged;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(DisinfectThresholdDiagram.OnReleaseHandle))]
    // ReSharper disable once UnusedMember.Local
    private static void OnReleaseHandle() => TriggerEvent();

    [HarmonyPostfix]
    [HarmonyPatch(nameof(DisinfectThresholdDiagram.ReceiveValueFromSlider))]
    // ReSharper disable once UnusedMember.Local
    private static void ReceiveValueFromSlider() => TriggerEvent();

    [HarmonyPostfix]
    [HarmonyPatch(nameof(DisinfectThresholdDiagram.ReceiveValueFromInput))]
    // ReSharper disable once UnusedMember.Local
    private static void ReceiveValueFromInput() => TriggerEvent();

    [HarmonyPostfix]
    [HarmonyPatch(nameof(DisinfectThresholdDiagram.OnClickToggle))]
    // ReSharper disable once UnusedMember.Local
    private static void OnClickToggle() => TriggerEvent();

    private static void TriggerEvent() => PatchControl.RunIfEnabled(
        () => {
            DiseaseSettingsChanged?.Invoke(
                SaveGame.Instance.minGermCountForDisinfect,
                SaveGame.Instance.enableAutoDisinfect
            );
        }
    );
}
