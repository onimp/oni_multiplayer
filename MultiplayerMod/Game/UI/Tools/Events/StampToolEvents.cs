using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.ModRuntime.Context;
using UnityEngine;

namespace MultiplayerMod.Game.UI.Tools.Events;

[HarmonyPatch(typeof(StampTool))]
[HarmonyPriority(Priority.High)]
public static class StampToolEvents {

    public static event Action<StampEventArgs>? Stamp;

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(StampTool.Stamp))]
    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<CodeInstruction> StampTranspiler(IEnumerable<CodeInstruction> instructions) {
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

    [RequireExecutionLevel(ExecutionLevel.Gameplay)]
    private static void OnStamp(StampTool instance, Vector2 location) => Stamp?.Invoke(
        new StampEventArgs(instance.stampTemplate, location)
    );

}
