using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using UnityEngine;

namespace MultiplayerMod.Game.UI.Tools.Events;

[HarmonyPatch(typeof(StampTool))]
public static class StampToolEvents {

    public static event EventHandler<StampEventArgs> Stamp;

    [HarmonyPriority(Priority.High)]
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(StampTool.Stamp))]
    private static IEnumerable<CodeInstruction> StampTranspiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator
    ) {
        using var source = instructions.GetEnumerator();
        var result = new List<CodeInstruction>();

        var templateLoaderStampMethod = AccessTools.Method(typeof(TemplateLoader), nameof(TemplateLoader.Stamp));
        result.AddConditional(source, it => it.Calls(templateLoaderStampMethod));

        // Add StampToolEvents.OnStamp(this, pos) after TemplateLoader.Stamp
        result.Add(new CodeInstruction(OpCodes.Ldarg_0)); // this
        result.Add(new CodeInstruction(OpCodes.Ldarg_1)); // pos
        result.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(StampToolEvents), nameof(OnStamp))));

        result.AddConditional(source, _ => false);

        return result;
    }

    private static void OnStamp(StampTool instance, Vector2 location) => PatchControl.RunIfEnabled(
        () => {
            Stamp?.Invoke(
                null,
                new StampEventArgs {
                    Template = instance.stampTemplate,
                    Location = location
                }
            );
        }
    );

}
