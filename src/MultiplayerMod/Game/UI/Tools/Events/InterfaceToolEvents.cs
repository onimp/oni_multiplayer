using System;
using System.Linq;
using HarmonyLib;
using MultiplayerMod.ModRuntime.Context;
using UnityEngine;

namespace MultiplayerMod.Game.UI.Tools.Events;

[HarmonyPatch(typeof(InterfaceTool))]
public static class InterfaceToolEvents {

    public static event Action<Vector2, string?>? MouseMoved;

    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    [HarmonyPrefix]
    [HarmonyPatch(nameof(InterfaceTool.OnMouseMove))]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    private static void OnMouseMove(Vector3 cursor_pos) {
        var kScreens = KScreenManager.Instance.screenStack.Where(screen => screen.mouseOver).ToList();
        MouseMoved?.Invoke(
            new Vector2(cursor_pos.x, cursor_pos.y),
            GetScreenName(kScreens.FirstOrDefault())
        );
    }

    private static string? GetScreenName(KScreen? screen) {
        switch (screen) {
            case TableScreen tableScreen:
                return tableScreen.title.ToLower();
            case RootMenu rootMenu:
                return rootMenu.detailsScreen.displayName;
            default:
                return screen?.displayName;
        }
    }
}
