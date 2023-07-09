using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Game.UI.Screens.Events;

public static class PrioritiesScreenEvents {

    public static event Action<ChoreConsumer, ChoreGroup, int> Set;
    public static event Action<ChoreGroup, int> DefaultSet;
    public static event Action<bool> AdvancedSet;

    [HarmonyPatch(typeof(ChoreConsumer))]
    // ReSharper disable once UnusedType.Local
    private static class ChoreConsumerEvents {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ChoreConsumer.SetPersonalPriority))]
        // ReSharper disable once UnusedMember.Local
        private static void SetPersonalPriority(ChoreConsumer __instance, ChoreGroup group, int value) =>
            PatchControl.RunIfEnabled(() => Set?.Invoke(__instance, group, value));
    }

    [HarmonyPatch(typeof(Immigration))]
    // ReSharper disable once UnusedType.Local
    private static class ImmigrationEvents {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Immigration.SetPersonalPriority))]
        // ReSharper disable once UnusedMember.Local
        private static void SetPersonalPriority(ChoreGroup group, int value) =>
            PatchControl.RunIfEnabled(() => DefaultSet?.Invoke(group, value));

    }

    [HarmonyPatch(typeof(JobsTableScreen))]
    // ReSharper disable once UnusedType.Local
    private static class JobsTableScreenEvents {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(JobsTableScreen.OnAdvancedModeToggleClicked))]
        // ReSharper disable once UnusedMember.Local
        private static void OnAdvancedModeToggleClicked() =>
            PatchControl.RunIfEnabled(
                () => AdvancedSet?.Invoke(global::Game.Instance.advancedPersonalPriorities)
            );

    }
}
