using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.ModRuntime.Context;

namespace MultiplayerMod.Multiplayer.UI.Diagnostics;

[UsedImplicitly]
[HarmonyPatch(typeof(ColonyDiagnosticUtility))]
internal static class ColonyDiagnosticUtilityPatch {

    [UsedImplicitly]
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ColonyDiagnosticUtility.AddWorld))]
    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    private static void AddWorldPostfix(ColonyDiagnosticUtility __instance, int worldID) {
        var colonyDiagnostic = new MultiplayerColonyDiagnostic(worldID);
        __instance.worldDiagnostics[worldID].Add(colonyDiagnostic);
        if (!__instance.diagnosticDisplaySettings[worldID].ContainsKey(colonyDiagnostic.id))
            __instance.diagnosticDisplaySettings[worldID].Add(
                colonyDiagnostic.id,
                ColonyDiagnosticUtility.DisplaySetting.AlertOnly
            );
        if (!__instance.diagnosticCriteriaDisabled[worldID].ContainsKey(colonyDiagnostic.id))
            __instance.diagnosticCriteriaDisabled[worldID].Add(colonyDiagnostic.id, new List<string>());
    }

}
