using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using UnityEngine;

namespace MultiplayerMod.Game.UI.Tools.Events;

[HarmonyPatch(typeof(InterfaceTool))]
public static class InterfaceToolEvents {

    public static event Action<Vector2>? MouseMoved;

    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    [HarmonyPrefix]
    [HarmonyPatch(nameof(InterfaceTool.OnMouseMove))]
    private static void OnMouseMove(Vector3 cursor_pos) =>
        PatchControl.RunIfEnabled(() => MouseMoved?.Invoke(new Vector2(cursor_pos.x, cursor_pos.y)));
}
