using System;
using HarmonyLib;
using MultiplayerMod.ModRuntime.Context;
using UnityEngine;

namespace MultiplayerMod.Game.UI.Tools.Events;

[HarmonyPatch(typeof(InterfaceTool))]
public static class InterfaceToolEvents {

    public static event Action<Vector2>? MouseMoved;

    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    [HarmonyPrefix]
    [HarmonyPatch(nameof(InterfaceTool.OnMouseMove))]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    private static void OnMouseMove(Vector3 cursor_pos) => MouseMoved?.Invoke(new Vector2(cursor_pos.x, cursor_pos.y));

}
