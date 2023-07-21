using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.UI.SideScreens;

[HarmonyPatch(typeof(CritterSensorSideScreen))]
public static class CritterSensorSideScreenEvents {

    public static event Action<ComponentReference, CritterSensorSideScreenEventArgs>? UpdateCritterSensor;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(CritterSensorSideScreen.ToggleCritters))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void ToggleCritters(CritterSensorSideScreen __instance) => TriggerEvent(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(nameof(CritterSensorSideScreen.ToggleEggs))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void ToggleEggs(CritterSensorSideScreen __instance) => TriggerEvent(__instance);

    private static void TriggerEvent(CritterSensorSideScreen instance) => PatchControl.RunIfEnabled(
        () => UpdateCritterSensor?.Invoke(
            instance.targetSensor.GetReference(),
            new CritterSensorSideScreenEventArgs(
                instance.targetSensor.countCritters,
                instance.targetSensor.countEggs
            )
        )
    );

    [Serializable]
    public record CritterSensorSideScreenEventArgs(
        bool CountCritters,
        bool CountEggs
    );
}
