using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Context;

namespace MultiplayerMod.Multiplayer.Patches;

[UsedImplicitly]
[HarmonyPatch(typeof(KModalButtonMenu))]
public class KModalButtonMenuPatch {

    [HarmonyTranspiler, UsedImplicitly]
    [HarmonyPatch("OnShow")]
    private static IEnumerable<CodeInstruction> OnShow(IEnumerable<CodeInstruction> instructions) {
        using var source = instructions.GetEnumerator();
        var result = new List<CodeInstruction>();
        var instanceGetter = AccessTools.PropertyGetter(
            typeof(SpeedControlScreen),
            nameof(SpeedControlScreen.Instance)
        );

        result.AddConditional(source, it => it.Calls(instanceGetter));
        result.AddConditional(source, it => it.Branches(out _));
        var jumpOutOfSpeedControlBlock = result.Last();

        result.Add(CodeInstruction.Call(typeof(KModalButtonMenuPatch), nameof(ShouldPause)));
        result.Add(jumpOutOfSpeedControlBlock);

        result.AddConditional(source, _ => false);
        return result;
    }

    private static bool ShouldPause() => !Runtime.Instance.Dependencies.Get<ExecutionLevelManager>()
        .LevelIsActive(ExecutionLevel.Game);

}
