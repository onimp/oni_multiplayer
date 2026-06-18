using HarmonyLib;
using JetBrains.Annotations;

namespace MultiplayerMod.Test.GameRuntime.Patches;

/// <summary>
/// Patches Sensors.Add to prevent immediate sensor.Update() call.
/// In the real game, Sensors.Add() calls sensor.Update() right away, but in the test
/// environment many sensors (MingleCellSensor, ClosestEdibleSensor, etc.) crash because
/// their dependencies (ScheduleManager, FetchManager internals, etc.) aren't fully initialized.
/// This patch replaces Add() with a simple list append — sensors will be updated when
/// explicitly triggered by the test code.
/// </summary>
[UsedImplicitly]
[HarmonyPatch(typeof(Sensors))]
public class SensorsPatch {

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Sensors.Add))]
    private static bool Sensors_Add_Prefix(Sensors __instance, Sensor sensor) {
        __instance.sensors.Add(sensor);
        return false; // Skip original (which calls sensor.Update())
    }

}
