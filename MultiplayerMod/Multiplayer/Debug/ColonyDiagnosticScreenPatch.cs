using HarmonyLib;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Multiplayer.Debug;

[HarmonyPatch(typeof(ColonyDiagnosticScreen))]
public class ColonyDiagnosticScreenPatch {

    [HarmonyPrefix]
    [HarmonyPatch(nameof(ColonyDiagnosticScreen.SpawnTrackerLines))]
    public static void SpawnTrackerLines(ColonyDiagnosticScreen __instance, int world) {
        if (MultiplayerState.Role == MultiplayerRole.None)
            return;

        __instance.AddDiagnostic<MultiplayerColonyDiagnostic>(
            world,
            __instance.contentContainer,
            __instance.diagnosticRows
        );
    }

}
