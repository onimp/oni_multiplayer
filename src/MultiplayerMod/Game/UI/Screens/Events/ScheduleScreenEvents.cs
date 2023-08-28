using System;
using System.Collections.Generic;
using HarmonyLib;
using MultiplayerMod.ModRuntime.Context;

namespace MultiplayerMod.Game.UI.Screens.Events;

public static class ScheduleScreenEvents {
    public static event Action<List<Schedule>>? Changed;

    [HarmonyPatch(typeof(ScheduleScreen))]
    // ReSharper disable once UnusedType.Local
    private static class ScheduleScreenPatch {

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ScheduleScreen.OnAddScheduleClick))]
        [RequireExecutionLevel(ExecutionLevel.Gameplay)]
        // ReSharper disable once UnusedMember.Local
        private static void OnAddScheduleClick() => Changed?.Invoke(ScheduleManager.Instance.schedules);

    }

    [HarmonyPatch(typeof(ScheduleScreenEntry))]
    // ReSharper disable once UnusedType.Local
    private static class ScheduleScreenEntryPatch {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ScheduleScreenEntry.OnDeleteClicked))]
        [RequireExecutionLevel(ExecutionLevel.Gameplay)]
        // ReSharper disable once UnusedMember.Local
        private static void OnDeleteClicked() => Changed?.Invoke(ScheduleManager.Instance.schedules);

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ScheduleScreenEntry.OnScheduleChanged))]
        [RequireExecutionLevel(ExecutionLevel.Gameplay)]
        // ReSharper disable once UnusedMember.Local
        private static void OnScheduleChanged() => Changed?.Invoke(ScheduleManager.Instance.schedules);

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ScheduleScreenEntry.OnNameChanged))]
        [RequireExecutionLevel(ExecutionLevel.Gameplay)]
        // ReSharper disable once UnusedMember.Local
        private static void OnNameChanged() => Changed?.Invoke(ScheduleManager.Instance.schedules);

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ScheduleScreenEntry.OnAlarmClicked))]
        [RequireExecutionLevel(ExecutionLevel.Gameplay)]
        // ReSharper disable once UnusedMember.Local
        private static void OnAlarmClicked() => Changed?.Invoke(ScheduleManager.Instance.schedules);

    }

}
