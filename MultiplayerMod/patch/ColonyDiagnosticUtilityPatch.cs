using System;
using System.Collections.Generic;
using HarmonyLib;
using MultiplayerMod.multiplayer;
using MultiplayerMod.oni;

namespace MultiplayerMod.patch
{
    [HarmonyPatch(typeof(ColonyDiagnosticUtility), nameof(ColonyDiagnosticUtility.AddWorld))]
    public class ColonyDiagnosticUtilityPatch
    {
        public static void Postfix(ColonyDiagnosticUtility __instance, int worldID)
        {
            if (!MultiplayerState.IsConnected) return;
            var colonyDiagnostic = new MultiplayerColonyDiagnostic(worldID);
            __instance.GetPrivateField<Dictionary<int, List<ColonyDiagnostic>>>("worldDiagnostics")[worldID]
                .Add(colonyDiagnostic);
            if (!__instance.diagnosticDisplaySettings[worldID].ContainsKey(colonyDiagnostic.id))
                __instance.diagnosticDisplaySettings[worldID].Add(colonyDiagnostic.id,
                    ColonyDiagnosticUtility.DisplaySetting.AlertOnly);
            if (!__instance.diagnosticCriteriaDisabled[worldID].ContainsKey(colonyDiagnostic.id))
                __instance.diagnosticCriteriaDisabled[worldID].Add(colonyDiagnostic.id, new List<string>());
        }
    }

    [HarmonyPatch(typeof(ColonyDiagnosticScreen), "SpawnTrackerLines")]
    public class ColonyDiagnosticScreenPatch
    {
        public static void Prefix(ColonyDiagnosticScreen __instance, int world)
        {
            if (!MultiplayerState.IsConnected) return;
            __instance.InvokePrivate("AddDiagnostic", new Type[] { typeof(MultiplayerColonyDiagnostic) },
                world,
                __instance.contentContainer,
                __instance.GetPrivateField<object>("diagnosticRows"));
        }
    }
}