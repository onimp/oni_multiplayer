using System;
using System.Collections.Generic;
using HarmonyLib;

namespace DedicatedServer.Game.Patches;

/// <summary>
/// Fixes ChoreConsumer in headless mode.
///
/// Two known failure modes:
///  1. Minions (choreTable == null): consumerState ctor NPEs at
///     schedulable.GetSchedule().GetCurrentScheduleBlock() because MinionIdentity.OnSpawn()
///     (which triggers OnAddDupe → schedule assignment) runs AFTER ChoreConsumer.OnSpawn()
///     in the BaseMinionConfig component order.
///     Fix: Prefix pre-assigns default schedule before OnSpawn runs.
///
///  2. Creatures (choreTable != null): ChoreTableChore.ctor → def.CreateSMI() →
///     StateMachineManager.CreateSMIFromDef() throws in headless → OnSpawn throws →
///     consumerState stays null.
///     Fix: FindNextChore Prefix guards against null consumerState (returns false=no chore).
///
/// NOTE: [HarmonyFinalizer] is NOT used here — the Harmony version bundled with the game
/// deadlocks during patching when a Finalizer is present.
/// </summary>
[HarmonyPatch(typeof(ChoreConsumer), "OnSpawn")]
public static class ChoreConsumerOnSpawnPatch {

    /// <summary>
    /// Pre-assign a default schedule to unscheduled Schedulables before ChoreConsumer
    /// runs, so ChoreConsumerState ctor doesn't NPE on GetSchedule().
    /// </summary>
    [HarmonyPrefix]
    static void EnsureScheduleAssigned(ChoreConsumer __instance) {
        try {
            if (ScheduleManager.Instance == null) return;
            var schedulable = __instance.GetComponent<Schedulable>();
            if (schedulable == null) return;
            if (ScheduleManager.Instance.GetSchedule(schedulable) != null) return;

            List<Schedule> schedules = ScheduleManager.Instance.GetSchedules();
            if (schedules == null || schedules.Count == 0) return;

            schedules[0].Assign(schedulable);
        } catch (Exception ex) {
            Console.WriteLine($"[BrainFix] EnsureSchedule failed on {__instance.name}: {ex.GetBaseException().Message}");
        }
    }
}

/// <summary>
/// Guards Brain.UpdateBrain() → FindNextChore() against null consumerState.
/// When ChoreConsumer.OnSpawn() throws (e.g. creature ChoreTable SMI creation fails),
/// TriggerLifecycle catches it and continues — but consumerState stays null.
/// Without this guard, the Brain NPEs every tick at consumerState.Refresh().
/// </summary>
[HarmonyPatch(typeof(ChoreConsumer), "FindNextChore")]
public static class ChoreConsumerFindNextChorePatch {

    [HarmonyPrefix]
    static bool GuardNullConsumerState(ChoreConsumer __instance, ref bool __result) {
        if (__instance.consumerState != null) return true; // run original
        __result = false; // no chore found
        return false;     // skip original
    }
}
