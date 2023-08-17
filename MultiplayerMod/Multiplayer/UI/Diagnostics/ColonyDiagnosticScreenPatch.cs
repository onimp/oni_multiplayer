using HarmonyLib;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Multiplayer.UI.Diagnostics;

[HarmonyPatch(typeof(ColonyDiagnosticScreen))]
// ReSharper disable once UnusedType.Global
public class ColonyDiagnosticScreenPatch {

    [HarmonyPrefix]
    [HarmonyPatch(nameof(ColonyDiagnosticScreen.SpawnTrackerLines))]
    // ReSharper disable once UnusedMember.Global
    public static void SpawnTrackerLines(ColonyDiagnosticScreen __instance, int world) {
        if (Dependencies.Get<MultiplayerGame>().Role == MultiplayerRole.None)
            return;

        __instance.AddDiagnostic<MultiplayerColonyDiagnostic>(
            world,
            __instance.contentContainer,
            __instance.diagnosticRows
        );
    }

}
