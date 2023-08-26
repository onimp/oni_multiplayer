using System;
using HarmonyLib;
using MultiplayerMod.ModRuntime.Context;

namespace MultiplayerMod.Game.UI.Screens.Events;

public static class PrioritiesScreenEvents {

    public static event Action<ChoreConsumer, ChoreGroup, int>? Set;
    public static event Action<ChoreGroup, int>? DefaultSet;
    public static event Action<bool>? AdvancedSet;

    [HarmonyPatch(typeof(ChoreConsumer))]
    // ReSharper disable once UnusedType.Local
    private static class ChoreConsumerEvents {

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ChoreConsumer.SetPersonalPriority))]
        [RequireExecutionLevel(ExecutionLevel.Runtime)]
        // ReSharper disable once UnusedMember.Local
        private static void SetPersonalPriority(ChoreConsumer __instance, ChoreGroup group, int value) =>
            Set?.Invoke(__instance, group, value);
    }

    [HarmonyPatch(typeof(Immigration))]
    // ReSharper disable once UnusedType.Local
    private static class ImmigrationEvents {

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Immigration.SetPersonalPriority))]
        [RequireExecutionLevel(ExecutionLevel.Runtime)]
        // ReSharper disable once UnusedMember.Local
        private static void SetPersonalPriority(ChoreGroup group, int value) =>
            DefaultSet?.Invoke(group, value);

    }

    [HarmonyPatch(typeof(JobsTableScreen))]
    // ReSharper disable once UnusedType.Local
    private static class JobsTableScreenEvents {

        [HarmonyPostfix]
        [HarmonyPatch(nameof(JobsTableScreen.OnAdvancedModeToggleClicked))]
        [RequireExecutionLevel(ExecutionLevel.Runtime)]
        // ReSharper disable once UnusedMember.Local
        private static void OnAdvancedModeToggleClicked() =>
            AdvancedSet?.Invoke(global::Game.Instance.advancedPersonalPriorities);

    }

}
