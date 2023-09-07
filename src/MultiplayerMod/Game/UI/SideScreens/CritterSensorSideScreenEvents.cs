using System;
using HarmonyLib;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.UI.SideScreens;

[HarmonyPatch(typeof(CritterSensorSideScreen))]
public static class CritterSensorSideScreenEvents {

    public static event Action<CritterSensorSideScreenEventArgs>? UpdateCritterSensor;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(CritterSensorSideScreen.ToggleCritters))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void ToggleCritters(CritterSensorSideScreen __instance) => TriggerEvent(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(nameof(CritterSensorSideScreen.ToggleEggs))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void ToggleEggs(CritterSensorSideScreen __instance) => TriggerEvent(__instance);

    [RequireExecutionLevel(ExecutionLevel.Game)]
    private static void TriggerEvent(CritterSensorSideScreen instance) => UpdateCritterSensor?.Invoke(
        new CritterSensorSideScreenEventArgs(
            instance.targetSensor.GetReference(),
            instance.targetSensor.countCritters,
            instance.targetSensor.countEggs
        )
    );

    [Serializable]
    public record CritterSensorSideScreenEventArgs(
        ComponentReference<LogicCritterCountSensor> Target,
        bool CountCritters,
        bool CountEggs
    );

}
