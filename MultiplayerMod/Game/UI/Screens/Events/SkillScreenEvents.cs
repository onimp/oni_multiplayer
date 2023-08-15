using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Game.UI.Screens.Events;

public static class SkillScreenEvents {

    public static event Action<MinionIdentity, string?>? SetHat;
    public static event Action<MinionIdentity, string>? MasterSkill;

    [HarmonyPatch(typeof(SkillsScreen))]
    // ReSharper disable once UnusedType.Local
    private static class SkillsScreenEvents {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(SkillsScreen.OnHatDropEntryClick))]
        // ReSharper disable once UnusedMember.Local
        private static void OnHatDropEntryClick(SkillsScreen __instance, IListableOption skill) =>
            PatchControl.RunIfEnabled(
                () => {
                    __instance.GetMinionIdentity(__instance.currentlySelectedMinion, out var minionIdentity, out _);
                    SetHat?.Invoke(
                        minionIdentity,
                        (skill as SkillListable)?.skillHat
                    );
                }
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
                () => {
                    __instance.skillsScreen.GetMinionIdentity(
                        __instance.assignableIdentity,
                        out var minionIdentity,
                        out _
                    );
                    SetHat?.Invoke(
                        minionIdentity,
                        (skill as SkillListable)?.skillHat
                    );
                }
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
                () => {
                    __instance.skillsScreen.GetMinionIdentity(
                        __instance.skillsScreen.CurrentlySelectedMinion,
                        out var minionIdentity,
                        out _
                    );
                    MasterSkill?.Invoke(
                        minionIdentity,
                        __instance.skillID
                    );
                }
            );

    }
}
