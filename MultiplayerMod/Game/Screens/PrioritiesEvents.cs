using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Game.Screens;

public static class PrioritiesEvents {

    public static event Action<string, string, int> PersonalPrioritySet;
    public static event Action<bool> PersonalPrioritiesAdvancedSet;

    [HarmonyPatch(typeof(ChoreConsumer))]
    // ReSharper disable once UnusedType.Global
    public static class ChoreConsumerEvents {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ChoreConsumer.SetPersonalPriority))]
        // ReSharper disable once UnusedMember.Local
        private static void SetPersonalPriority(ChoreConsumer __instance, ChoreGroup group, int value) =>
            PatchControl.RunIfEnabled(() => PersonalPrioritySet?.Invoke(__instance.GetProperName(), group.Id, value));
    }

    [HarmonyPatch(typeof(Immigration))]
    // ReSharper disable once UnusedType.Global
    public static class ImmigrationEvents {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Immigration.SetPersonalPriority))]
        // ReSharper disable once UnusedMember.Local
        private static void SetPersonalPriority(ChoreGroup group, int value) =>
            PatchControl.RunIfEnabled(() => PersonalPrioritySet?.Invoke(null, group.Id, value));

    }

    [HarmonyPatch(typeof(JobsTableScreen))]
    // ReSharper disable once UnusedType.Global
    public static class JobsTableScreenEvents {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(JobsTableScreen.OnAdvancedModeToggleClicked))]
        // ReSharper disable once UnusedMember.Local
        private static void OnAdvancedModeToggleClicked() =>
            PatchControl.RunIfEnabled(
                () => PersonalPrioritiesAdvancedSet?.Invoke(global::Game.Instance.advancedPersonalPriorities)
            );

    }
}
