using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

namespace MultiplayerMod.Test.GameRuntime.Patches;

/// <summary>
/// Patches ChoreConsumerState constructor to handle null Schedulable/Schedule.
/// In the real game, every minion's Schedulable always has a Schedule assigned.
/// In tests, Schedulable may not exist or may have no Schedule, causing NPE at:
///   schedulable.GetSchedule().GetCurrentScheduleBlock()
/// when GetSchedule() returns null.
///
/// This patch sets the schedulable field to null before the constructor runs,
/// so the existing null-check in the constructor (if schedulable != null) skips
/// the schedule block resolution entirely.
/// </summary>
[UsedImplicitly]
[HarmonyPatch(typeof(ChoreConsumerState), MethodType.Constructor, typeof(ChoreConsumer))]
public class ChoreConsumerStatePatch {

    [UsedImplicitly]
    [HarmonyPostfix]
    private static void Postfix(ChoreConsumerState __instance) {
        // If scheduleBlock is null (because GetSchedule() returned null and the
        // original code crashed), just clear it. But since we use a Prefix to prevent
        // the crash, this is a safety net.
    }

    [UsedImplicitly]
    [HarmonyPrefix]
    private static bool Prefix(ChoreConsumerState __instance, ChoreConsumer consumer) {
        // Replicate the constructor but with null-safe schedule handling.
        // This prevents NPE when schedulable.GetSchedule() returns null.
        __instance.consumer = consumer;
        __instance.navigator = consumer.GetComponent<Navigator>();
        __instance.prefabid = consumer.GetComponent<KPrefabID>();

        var ownableField = typeof(ChoreConsumerState).GetField("ownable", BindingFlags.Public | BindingFlags.Instance);
        ownableField?.SetValue(__instance, consumer.GetComponent<Ownable>());

        __instance.gameObject = consumer.gameObject;
        __instance.solidTransferArm = consumer.GetComponent<SolidTransferArm>();
        __instance.hasSolidTransferArm = __instance.solidTransferArm != null;
        __instance.resume = consumer.GetComponent<MinionResume>();
        __instance.choreDriver = consumer.GetComponent<ChoreDriver>();
        __instance.schedulable = consumer.GetComponent<Schedulable>();
        __instance.traits = consumer.GetComponent<Klei.AI.Traits>();
        __instance.choreProvider = consumer.GetComponent<ChoreProvider>();

        var identity = consumer.GetComponent<MinionIdentity>();
        if (identity != null) {
            if (identity.assignableProxy == null || identity.assignableProxy.Get() == null) {
                // In tests, assignableProxy may already be initialized
                try {
                    __instance.assignables = identity.GetSoleOwner();
                    __instance.equipment = identity.GetEquipment();
                } catch {
                    __instance.assignables = consumer.GetComponent<Assignables>();
                    __instance.equipment = consumer.GetComponent<Equipment>();
                }
            } else {
                __instance.assignables = identity.GetSoleOwner();
                __instance.equipment = identity.GetEquipment();
            }
        } else {
            __instance.assignables = consumer.GetComponent<Assignables>();
            __instance.equipment = consumer.GetComponent<Equipment>();
        }

        __instance.storage = consumer.GetComponent<Storage>();
        __instance.consumableConsumer = consumer.GetComponent<ConsumableConsumer>();
        __instance.worker = consumer.GetComponent<WorkerBase>();
        __instance.selectable = consumer.GetComponent<KSelectable>();

        // THE FIX: null-safe schedule block resolution
        if (__instance.schedulable != null) {
            var schedule = __instance.schedulable.GetSchedule();
            if (schedule != null) {
                __instance.scheduleBlock = schedule.GetCurrentScheduleBlock();
            }
        }

        return false; // Skip original constructor
    }

}
