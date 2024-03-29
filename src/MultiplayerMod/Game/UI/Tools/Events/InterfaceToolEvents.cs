﻿using System;
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
                WorldToScreen(kScreen, cursor_pos),
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

    private static Vector2? WorldToScreen(KScreen? screen, Vector2 cursorPos) {
        if (screen == null) return null;
        var screenRectTransform = screen.transform as RectTransform;
        if (screenRectTransform == null) return null;

        var screenPoint = Camera.main.WorldToScreenPoint(cursorPos);

        return new Vector2(
            (screenPoint.x - screenRectTransform.position.x) / screenRectTransform.rect.width,
            (screenPoint.y - screenRectTransform.position.y) / screenRectTransform.rect.height
        );
    }

    [Serializable]
    public record MouseMovedEventArgs(
        Vector2 Position,
        Vector2? PositionWithinScreen,
        string? ScreenName,
        Type? ScreenType
    );
}
