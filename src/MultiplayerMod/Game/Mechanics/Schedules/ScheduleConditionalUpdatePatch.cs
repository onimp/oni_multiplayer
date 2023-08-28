using HarmonyLib;

namespace MultiplayerMod.Game.Mechanics.Schedules;

[HarmonyPatch(typeof(Schedule))]
public static class ScheduleConditionalUpdatePatch {

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Schedule.SetGroup))]
    private static bool SetGroupPrefix(Schedule __instance, int idx, ScheduleGroup group) {
        if (idx < 0 || idx >= __instance.blocks.Count)
            return false;

        return __instance.blocks[idx].GroupId != group.Id;
    }

}
