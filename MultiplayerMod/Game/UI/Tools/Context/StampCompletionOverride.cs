using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Game.Context;
using UnityEngine;

namespace MultiplayerMod.Game.UI.Tools.Context;

[HarmonyPatch]
[HarmonyPriority(Priority.Low)]
public class StampCompletionOverride : IGameContext {

    private static bool enabled;

    public void Apply() {
        enabled = true;
    }

    public void Restore() {
        enabled = false;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(StampTool), nameof(StampTool.Stamp))]
    private static IEnumerable<CodeInstruction> StampTranspiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator
    ) {
        using var source = instructions.GetEnumerator();
        var result = new List<CodeInstruction>();

        var templateLoaderStampMethod = AccessTools.Method(typeof(TemplateLoader), nameof(TemplateLoader.Stamp));
        var stampToolReadyField = AccessTools.Field(typeof(StampTool), nameof(StampTool.ready));
        var speedControlScreenInstanceGetter = AccessTools.PropertyGetter(
            typeof(SpeedControlScreen),
            nameof(SpeedControlScreen.Instance)
        );
        var speedControlScreenIsPausedGetter = AccessTools.PropertyGetter(
            typeof(SpeedControlScreen),
            nameof(SpeedControlScreen.IsPaused)
        );
        var templateLoaderStampOverrideMethod = AccessTools.Method(
            typeof(StampCompletionOverride),
            nameof(TemplateLoaderStampOverride)
        );

        var pauseOnComplete = generator.DeclareLocal(typeof(bool));

        result.AddConditional(source, it => it.StoresField(stampToolReadyField));

        // Store pauseOnComplete
        result.Add(new CodeInstruction(OpCodes.Call, speedControlScreenInstanceGetter));
        result.Add(new CodeInstruction(OpCodes.Callvirt, speedControlScreenIsPausedGetter));
        result.Add(new CodeInstruction(OpCodes.Stloc_S, pauseOnComplete));

        result.AddConditional(source, it => it.Calls(templateLoaderStampMethod));

        // Replace TemplateLoader.Stamp call
        var templateLoaderStampCall = result.GetRange(result.Count - 7, 7);
        result.RemoveRange(result.Count - 7, 7);
        result.Add(new CodeInstruction(OpCodes.Ldarg_0) { labels = templateLoaderStampCall[0].labels }); // this
        result.Add(new CodeInstruction(OpCodes.Ldarg_1)); // pos
        result.Add(new CodeInstruction(OpCodes.Ldloc_S, pauseOnComplete)); // pauseOnComplete
        result.Add(new CodeInstruction(OpCodes.Call, templateLoaderStampOverrideMethod));

        result.AddConditional(source, _ => false);

        return result;
    }

    private static void TemplateLoaderStampOverride(StampTool instance, Vector2 location, bool pauseOnComplete) {
        if (enabled)
            TemplateLoader.Stamp(instance.stampTemplate, location, () => StampCompleteCallback(pauseOnComplete));
        else
            TemplateLoader.Stamp(instance.stampTemplate, location, () => instance.CompleteStamp(pauseOnComplete));
    }

    private static void StampCompleteCallback(bool pauseOnComplete) {
        if (pauseOnComplete)
            SpeedControlScreen.Instance.Pause();
    }

}
