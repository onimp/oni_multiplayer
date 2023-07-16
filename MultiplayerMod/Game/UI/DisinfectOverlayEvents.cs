using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Game.UI;

[HarmonyPatch(typeof(DisinfectThresholdDiagram))]
public static class DiseaseOverlayEvents {

    public static event Action<int, bool>? DiseaseSettingsChanged;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(DisinfectThresholdDiagram.OnReleaseHandle))]
    // ReSharper disable once UnusedMember.Local
    private static void OnReleaseHandle() => PatchControl.RunIfEnabled(
        () => {
            DiseaseSettingsChanged?.Invoke(
                SaveGame.Instance.minGermCountForDisinfect,
                SaveGame.Instance.enableAutoDisinfect
            );
        }
    );

    [HarmonyPostfix]
    [HarmonyPatch(nameof(DisinfectThresholdDiagram.ReceiveValueFromSlider))]
    // ReSharper disable once UnusedMember.Local
    private static void ReceiveValueFromSlider() => PatchControl.RunIfEnabled(
        () => {
            DiseaseSettingsChanged?.Invoke(
                SaveGame.Instance.minGermCountForDisinfect,
                SaveGame.Instance.enableAutoDisinfect
            );
        }
    );

    [HarmonyPostfix]
    [HarmonyPatch(nameof(DisinfectThresholdDiagram.ReceiveValueFromInput))]
    // ReSharper disable once UnusedMember.Local
    private static void ReceiveValueFromInput() => PatchControl.RunIfEnabled(
        () => {
            DiseaseSettingsChanged?.Invoke(
                SaveGame.Instance.minGermCountForDisinfect,
                SaveGame.Instance.enableAutoDisinfect
            );
        }
    );

    [HarmonyPostfix]
    [HarmonyPatch(nameof(DisinfectThresholdDiagram.OnClickToggle))]
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Local
    private static void OnClickToggle() => PatchControl.RunIfEnabled(
        () => {
            DiseaseSettingsChanged?.Invoke(
                SaveGame.Instance.minGermCountForDisinfect,
                SaveGame.Instance.enableAutoDisinfect
            );
        }
    );
}
