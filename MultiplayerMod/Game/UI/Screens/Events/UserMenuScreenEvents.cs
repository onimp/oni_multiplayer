using System;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.UI.Screens.Events;

[HarmonyPatch(typeof(UserMenuScreen))]
public static class UserMenuScreenEvents {

    public static event Action<ComponentReference<Prioritizable>, PrioritySetting>? PriorityChanged;

    [UsedImplicitly]
    [HarmonyPostfix]
    [HarmonyPatch(nameof(UserMenuScreen.OnPriorityClicked))]
    [RequireExecutionLevel(ExecutionLevel.Gameplay)]
    private static void OnPriorityClicked(UserMenuScreen __instance, PrioritySetting priority) {
        if (__instance.selected == null)
            return;

        var component = __instance.selected.GetComponent<Prioritizable>();
        if (component == null)
            return;

        PriorityChanged?.Invoke(component.GetReference(), priority);
    }

}
