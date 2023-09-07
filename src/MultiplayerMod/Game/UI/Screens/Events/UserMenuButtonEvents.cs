using System;
using System.Linq;
using HarmonyLib;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using UnityEngine;

namespace MultiplayerMod.Game.UI.Screens.Events;

public static class UserMenuButtonEvents {

    public static event Action<GameObject, System.Action>? Click;

    private static readonly Type[] skipButtonTypes = {
        // Camera specific actions
        typeof(Navigator),
        // Should be synced as a tool
        typeof(MoveToLocationMonitor.Instance),
        // Synced as a tool
        typeof(CopyBuildingSettings)
    };

    [HarmonyPatch(typeof(UserMenu))]
    // ReSharper disable once UnusedType.Local
    private static class UserMenuEvents {

        [HarmonyPrefix]
        [HarmonyPatch(nameof(UserMenu.AddButton))]
        // ReSharper disable once UnusedMember.Local
        private static void AddButton(GameObject go, ref KIconButtonMenu.ButtonInfo button) {
            var original = button.onClick;
            if (original == null)
                return;
            if (skipButtonTypes.Contains(original.Method.DeclaringType)) return;

            button.onClick = () => {
                original.Invoke();
                Execution.RunIfLevelIsActive(
                    ExecutionLevel.Gameplay,
                    () => Click?.Invoke(go, original)
                );
            };
        }

    }
}
