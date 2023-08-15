using System;
using System.Collections.Generic;
using HarmonyLib;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Game.UI.Screens.Events;

public static class ScheduleScreenEvents {
    public static event Action<List<Schedule>>? Changed;

    [HarmonyPatch(typeof(ScheduleScreen))]
    // ReSharper disable once UnusedType.Local
    private static class ScheduleScreenPatch {

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ScheduleScreen.OnAddScheduleClick))]
        // ReSharper disable once UnusedMember.Local
        private static void OnAddScheduleClick() =>
            PatchControl.RunIfEnabled(() => Changed?.Invoke(ScheduleManager.Instance.schedules));

    }

    [HarmonyPatch(typeof(ScheduleScreenEntry))]
    // ReSharper disable once UnusedType.Local
    private static class ScheduleScreenEntryPatch {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ScheduleScreenEntry.OnDeleteClicked))]
        // ReSharper disable once UnusedMember.Local
        private static void OnDeleteClicked() =>
            PatchControl.RunIfEnabled(() => Changed?.Invoke(ScheduleManager.Instance.schedules));

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ScheduleScreenEntry.OnScheduleChanged))]
        // ReSharper disable once UnusedMember.Local
        private static void OnScheduleChanged() =>
            PatchControl.RunIfEnabled(() => Changed?.Invoke(ScheduleManager.Instance.schedules));

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ScheduleScreenEntry.OnNameChanged))]
        // ReSharper disable once UnusedMember.Local
        private static void OnNameChanged() =>
            PatchControl.RunIfEnabled(() => Changed?.Invoke(ScheduleManager.Instance.schedules));

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ScheduleScreenEntry.OnAlarmClicked))]
        // ReSharper disable once UnusedMember.Local
        private static void OnAlarmClicked() =>
            PatchControl.RunIfEnabled(() => Changed?.Invoke(ScheduleManager.Instance.schedules));

    }

}
