using System;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.ModRuntime.Context;
using UnityEngine;

namespace MultiplayerMod.Game.UI.Tools.Events;

[HarmonyPatch(typeof(InterfaceTool))]
public static class InterfaceToolEvents {

    public static event Action<MouseMovedEventArgs>? MouseMoved;

    // ReSharper disable once InconsistentNaming
    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch(nameof(InterfaceTool.OnMouseMove))]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    private static void OnMouseMove(Vector3 cursor_pos) {
        var kScreens = KScreenManager.Instance.screenStack.Where(screen => screen.mouseOver).ToList();
        var kScreen = kScreens.FirstOrDefault();
        MouseMoved?.Invoke(
            new MouseMovedEventArgs(
                new Vector2(cursor_pos.x, cursor_pos.y),
                GetScreenName(kScreen),
                kScreen?.GetType()
            )
        );
    }

    private static string? GetScreenName(KScreen? screen) => screen switch {
        TableScreen tableScreen => tableScreen.title.ToLower(),
        RootMenu rootMenu => rootMenu.detailsScreen.displayName,
        _ => screen?.displayName
    };

    [Serializable]
    public record MouseMovedEventArgs(
        Vector2 Position,
        string? ScreenName,
        Type? ScreenType
    );
}
