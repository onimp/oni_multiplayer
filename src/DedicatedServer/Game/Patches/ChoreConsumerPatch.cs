using System;
using System.Collections.Generic;
using HarmonyLib;

namespace DedicatedServer.Game.Patches;

/// <summary>
/// Ensures ChoreConsumer.OnSpawn always initializes consumerState, even if ChoreTable
/// construction or ChoreConsumerState initialization partially fails in headless mode.
///
/// Two known failure modes:
///  1. Minions (choreTable == null): consumerState ctor NPEs at
///     schedulable.GetSchedule().GetCurrentScheduleBlock() because MinionIdentity.OnSpawn()
///     (which triggers OnAddDupe → schedule assignment) runs AFTER ChoreConsumer.OnSpawn()
///     in the component order from BaseMinionConfig.
///
///  2. Creatures (choreTable != null): ChoreTableChore.ctor calls def.CreateSMI() which
///     calls StateMachineManager.CreateSMIFromDef(), which can fail in headless if the
///     state machine's CreateStates/BindStates/InitializeStateMachine has rendering deps.
/// </summary>
[HarmonyPatch(typeof(ChoreConsumer), "OnSpawn")]
public static class ChoreConsumerPatch {

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

    /// <summary>
    /// After OnSpawn (even if it threw): if consumerState is still null, initialize it.
    /// Suppresses the original exception — OnSpawn failure is non-fatal; the brain can
    /// still tick with a valid consumerState even if choreTableInstance is missing.
    /// </summary>
    [HarmonyFinalizer]
    static Exception EnsureConsumerState(Exception __exception, ChoreConsumer __instance) {
        if (__exception != null) {
            Console.WriteLine($"[BrainFix] ChoreConsumer.OnSpawn threw on {__instance.name}: {__exception.GetBaseException().Message}");
        }

        if (__instance.consumerState == null) {
            try {
                __instance.consumerState = new ChoreConsumerState(__instance);
                if (__exception != null)
                    Console.WriteLine($"[BrainFix] consumerState fallback OK for {__instance.name}");
            } catch (Exception e2) {
                Console.WriteLine($"[BrainFix] consumerState fallback also failed for {__instance.name}: {e2.GetBaseException().Message}");
            }
        }

        return null; // Suppress original exception
    }
}
