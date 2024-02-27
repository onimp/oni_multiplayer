using System;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.ModRuntime.Context;

namespace MultiplayerMod.Game.UI.Screens.Events;

[HarmonyPatch(typeof(MeterScreen))]
public class MeterScreenEvents {

    public static event Action<bool>? RedAlertToggling;

    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch(nameof(MeterScreen.OnRedAlertClick))]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    private static void BeforeRedAlertClick() => RedAlertToggling?.Invoke(
        !ClusterManager.Instance.activeWorld.AlertManager.IsRedAlertToggledOn()
    );

}
