using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using UnityEngine;

namespace MultiplayerMod.Game.Screens.Consumable;

[HarmonyPatch(typeof(ConsumablesTableScreen))]
public static class ConsumablesTableScreenEvents {

    public static event Action<List<Tag>> PermittedByDefault;

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

                PermittedByDefault?.Invoke(ConsumerManager.instance.DefaultForbiddenTagsList);
            }
        );

}
