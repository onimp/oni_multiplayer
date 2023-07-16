using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using UnityEngine;

namespace MultiplayerMod.Game.UI.Screens.Events;

public static class ConsumableScreenEvents {
    public static event Action<ConsumableConsumer, string, bool>? PermitToMinion;
    public static event Action<List<Tag>>? PermitByDefault;

    [HarmonyPatch(typeof(ConsumableConsumer))]
    private static class ConsumablesEvents {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ConsumableConsumer.SetPermitted))]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        // ReSharper disable once UnusedMember.Local
        private static void SetPermittedPatch(ConsumableConsumer __instance, string consumable_id, bool is_allowed) =>
            PatchControl.RunIfEnabled(
                () => PermitToMinion?.Invoke(__instance, consumable_id, is_allowed)
            );

    }

    [HarmonyPatch(typeof(ConsumablesTableScreen))]
    private static class ConsumablesTableScreenEvents {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ConsumablesTableScreen.set_value_consumable_info))]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        // ReSharper disable once UnusedMember.Local
        private static void SetPermittedPatch(ConsumablesTableScreen __instance, GameObject widget_go) =>
            PatchControl.RunIfEnabled(
                () => {
                    var widgetRow = __instance.rows.FirstOrDefault(
                        row => row.rowType != TableRow.RowType.WorldDivider && row.ContainsWidget(widget_go)
                    );
                    if (TableRow.RowType.Default != widgetRow?.rowType) return;

                    PermitByDefault?.Invoke(ConsumerManager.instance.DefaultForbiddenTagsList);
                }
            );
    }
}
