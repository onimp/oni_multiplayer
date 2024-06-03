using System;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.ModRuntime.Context;

namespace MultiplayerMod.Game.UI.Tools.Events;

[HarmonyPatch(typeof(MoveToLocationTool))]
public static class MoveToLocationToolEvents {

    public static event Action<Navigator?, Movable?, int>? LocationSet;

    [UsedImplicitly]
    [HarmonyPostfix]
    [HarmonyPatch(nameof(MoveToLocationTool.SetMoveToLocation))]
    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    private static void SetMoveToLocationPostfix(MoveToLocationTool __instance, int target_cell) =>
        LocationSet?.Invoke(__instance.targetNavigator, __instance.targetMovable, target_cell);

}
