using System;
using HarmonyLib;
using UnityEngine;

namespace MultiplayerMod.Game;

[HarmonyPatch(typeof(InterfaceTool))]
public static class InterfaceToolEvents {

    public static event Action<Vector2> MouseMoved;

    // ReSharper disable once InconsistentNaming
    [HarmonyPrefix]
    [HarmonyPatch(nameof(InterfaceTool.OnMouseMove))]
    private static void OnMouseMove(Vector3 cursor_pos) => MouseMoved?.Invoke(new Vector2(cursor_pos.x, cursor_pos.y));

}
