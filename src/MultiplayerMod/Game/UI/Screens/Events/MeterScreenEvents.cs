using System;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Game.World;
using MultiplayerMod.ModRuntime.Context;

namespace MultiplayerMod.Game.UI.Screens.Events;

[HarmonyPatch(typeof(MeterScreen))]
public class MeterScreenEvents {

    public static event Action<bool, int?>? RedAlertToggling;

    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch(nameof(MeterScreen.OnRedAlertClick))]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    private static void BeforeRedAlertClick() {
        var activeWorld = WorldIdentity.ActiveWorld;
        if (activeWorld == null)
            return;

        var redAlertToggledOn = WorldIdentity.IsRedAlertToggledOn(activeWorld);
        if (!redAlertToggledOn.HasValue)
            return;

        RedAlertToggling?.Invoke(!redAlertToggledOn.Value, WorldIdentity.GetWorldId(activeWorld));
    }

}
