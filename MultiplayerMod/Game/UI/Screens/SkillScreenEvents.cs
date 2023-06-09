using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Game.UI.Screens;

public static class SkillScreenEvents {

    public static event Action<string, string> HatSet;
    public static event Action<string, string> SkillMastered;

    [HarmonyPatch(typeof(SkillsScreen))]
    // ReSharper disable once UnusedType.Local
    private static class SkillsScreenEvents {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(SkillsScreen.OnHatDropEntryClick))]
        // ReSharper disable once UnusedMember.Local
        private static void OnHatDropEntryClick(SkillsScreen __instance, IListableOption skill) =>
            PatchControl.RunIfEnabled(
                () => HatSet?.Invoke(
                    __instance.currentlySelectedMinion.GetProperName(),
                    (skill as SkillListable)?.skillHat
                )
            );
    }

    [HarmonyPatch(typeof(SkillMinionWidget))]
    // ReSharper disable once UnusedType.Local
    private static class SkillMinionWidgetEvents {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(SkillMinionWidget.OnHatDropEntryClick))]
        // ReSharper disable once UnusedMember.Local
        private static void OnHatDropEntryClick(SkillMinionWidget __instance, IListableOption skill) =>
            PatchControl.RunIfEnabled(
                () => HatSet?.Invoke(
                    __instance.assignableIdentity.GetProperName(),
                    (skill as SkillListable)?.skillHat
                )
            );
    }

    [HarmonyPatch(typeof(SkillWidget))]
    // ReSharper disable once UnusedType.Local
    private static class SkillWidgetEvents {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(SkillWidget.OnPointerClick))]
        // ReSharper disable once UnusedMember.Local
        private static void OnPointerClick(SkillWidget __instance) =>
            PatchControl.RunIfEnabled(
                () => SkillMastered?.Invoke(
                    __instance.skillsScreen.CurrentlySelectedMinion.GetProperName(),
                    __instance.skillID
                )
            );

    }
}
