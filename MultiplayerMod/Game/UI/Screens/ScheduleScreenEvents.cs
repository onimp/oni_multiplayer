using System;
using System.Collections.Generic;
using HarmonyLib;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Game.UI.Screens;

public static class ScheduleScreenEvents {
    public static event Action<List<Schedule>> SchedulesChanged;

    static ScheduleScreenEvents() {
        ScheduleScreenEntryPatch.ScheduleChanged += schedules => SchedulesChanged?.Invoke(schedules);
        ScheduleScreenPatch.ScheduleAdded += schedules => SchedulesChanged?.Invoke(schedules);
    }

    [HarmonyPatch(typeof(ScheduleScreen))]
    private static class ScheduleScreenPatch {
        public static event Action<List<Schedule>> ScheduleAdded;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ScheduleScreen.OnAddScheduleClick))]
        // ReSharper disable once UnusedMember.Local
        private static void OnAddScheduleClick() =>
            PatchControl.RunIfEnabled(() => ScheduleAdded?.Invoke(ScheduleManager.Instance.schedules));

    }

    [HarmonyPatch(typeof(ScheduleScreenEntry))]
    private static class ScheduleScreenEntryPatch {
        public static event Action<List<Schedule>> ScheduleChanged;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ScheduleScreenEntry.OnDeleteClicked))]
        // ReSharper disable once UnusedMember.Local
        private static void OnDeleteClicked() =>
            PatchControl.RunIfEnabled(() => ScheduleChanged?.Invoke(ScheduleManager.Instance.schedules));

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ScheduleScreenEntry.OnScheduleChanged))]
        // ReSharper disable once UnusedMember.Local
        private static void OnScheduleChanged() =>
            PatchControl.RunIfEnabled(() => ScheduleChanged?.Invoke(ScheduleManager.Instance.schedules));

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ScheduleScreenEntry.OnNameChanged))]
        // ReSharper disable once UnusedMember.Local
        private static void OnNameChanged() =>
            PatchControl.RunIfEnabled(() => ScheduleChanged?.Invoke(ScheduleManager.Instance.schedules));

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ScheduleScreenEntry.OnAlarmClicked))]
        // ReSharper disable once UnusedMember.Local
        private static void OnAlarmClicked() =>
            PatchControl.RunIfEnabled(() => ScheduleChanged?.Invoke(ScheduleManager.Instance.schedules));

    }

}
