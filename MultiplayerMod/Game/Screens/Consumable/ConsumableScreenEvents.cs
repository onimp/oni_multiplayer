using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Game.Screens.Consumable;

[HarmonyPatch(typeof(ConsumableConsumer))]
public static class ConsumablesEvents {
    public static event Action<string, string, bool> PermittedToMinion;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ConsumableConsumer.SetPermitted))]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    // ReSharper disable once UnusedMember.Local
    private static void SetPermittedPatch(ConsumableConsumer __instance, string consumable_id, bool is_allowed) =>
        PatchControl.RunIfEnabled(
            () => PermittedToMinion?.Invoke(__instance.GetProperName(), consumable_id, is_allowed)
        );

}
