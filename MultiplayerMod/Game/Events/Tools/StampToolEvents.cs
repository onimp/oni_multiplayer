using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Game.Building;
using UnityEngine;

namespace MultiplayerMod.Game.Events.Tools;

[HarmonyPatch(typeof(StampTool))]
public static class StampToolEvents {

    public static event EventHandler<StampEventArgs> Stamp;

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(StampTool.Stamp))]
    private static IEnumerable<CodeInstruction> StampTranspiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator
    ) {
        using var source = instructions.GetEnumerator();
        var result = new List<CodeInstruction>();

        var templateLoaderStampMethod = AccessTools.Method(typeof(TemplateLoader), nameof(TemplateLoader.Stamp));

        result.AddConditional(
            source,
            it => it.StoresField(AccessTools.Field(typeof(StampTool), nameof(StampTool.ready)))
        );

        var pauseOnComplete = generator.DeclareLocal(typeof(bool));
        result.Add(
            new CodeInstruction(
                OpCodes.Call,
                AccessTools.PropertyGetter(typeof(SpeedControlScreen), nameof(SpeedControlScreen.Instance))
            )
        );
        result.Add(
            new CodeInstruction(
                OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(SpeedControlScreen), nameof(SpeedControlScreen.IsPaused))
            )
        );
        result.Add(new CodeInstruction(OpCodes.Stloc_S, pauseOnComplete));

        result.AddConditional(source, it => it.Calls(templateLoaderStampMethod));

        // Extract entire TemplateLoader.Stamp call
        var templateLoaderStampCall = result.GetRange(result.Count - 7, 7);
        result.RemoveRange(result.Count - 7, 7);

        var originalLabels = templateLoaderStampCall[0].labels;

        result.Add(new CodeInstruction(OpCodes.Ldarg_0) { labels = originalLabels }); // this
        result.Add(new CodeInstruction(OpCodes.Ldarg_1)); // pos
        result.Add(new CodeInstruction(OpCodes.Ldloc_S, pauseOnComplete)); // pauseOnComplete
        result.Add(
            new CodeInstruction(
                OpCodes.Call,
                AccessTools.Method(typeof(StampToolEvents), nameof(TemplateLoaderStampOverride))
            )
        );

        result.AddConditional(source, _ => false);

        return result;
    }

    private static void TemplateLoaderStampOverride(StampTool instance, Vector2 pos, bool pauseOnComplete) {
        if (StampCompletion.Override)
            TemplateLoader.Stamp(
                instance.stampTemplate,
                pos,
                () => {
                    if (pauseOnComplete)
                        SpeedControlScreen.Instance.Pause();
                }
            );
        else
            TemplateLoader.Stamp(instance.stampTemplate, pos, () => instance.CompleteStamp(pauseOnComplete));
        OnStamp(instance, pos);
    }

    private static void OnStamp(StampTool instance, Vector2 location) => PatchControl.RunIfEnabled(
        () => {
            var args = new StampEventArgs {
                Template = instance.stampTemplate,
                Location = location
            };
            Stamp?.Invoke(null, args);
        }
    );

}
