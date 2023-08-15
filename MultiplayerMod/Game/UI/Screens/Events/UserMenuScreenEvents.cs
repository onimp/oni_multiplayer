using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.UI.Screens.Events;

[HarmonyPatch(typeof(UserMenuScreen))]
// ReSharper disable once UnusedType.Global
public static class UserMenuScreenEvents {

    public static event Action<ComponentReference<Prioritizable>, PrioritySetting>? PriorityChanged;

    // ReSharper disable once UnusedMember.Global
    [HarmonyPostfix]
    [HarmonyPatch(nameof(UserMenuScreen.OnPriorityClicked))]
    private static void OnPriorityClicked(UserMenuScreen __instance, PrioritySetting priority) =>
        PatchControl.RunIfEnabled(
            () => {
                if (__instance.selected == null)
                    return;

                Prioritizable component = __instance.selected.GetComponent<Prioritizable>();
                if (component == null)
                    return;

                PriorityChanged?.Invoke(component.GetReference(), priority);
            }
        );

}
