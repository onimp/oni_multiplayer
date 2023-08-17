using System.Collections.Generic;
using HarmonyLib;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Multiplayer.UI.Diagnostics;

[HarmonyPatch(typeof(ColonyDiagnosticUtility))]
// ReSharper disable once UnusedType.Global
internal static class ColonyDiagnosticUtilityPatch {

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ColonyDiagnosticUtility.AddWorld))]
    // ReSharper disable once UnusedMember.Local
    private static void AddWorldPostfix(ColonyDiagnosticUtility __instance, int worldID) {
        if (Dependencies.Get<MultiplayerGame>().Role == MultiplayerRole.None)
            return;

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
