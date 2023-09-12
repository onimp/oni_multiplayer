using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.ModRuntime.Context;

namespace MultiplayerMod.Multiplayer.UI.Diagnostics;

[UsedImplicitly]
[HarmonyPatch(typeof(ColonyDiagnosticScreen))]
public class ColonyDiagnosticScreenPatch {

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ColonyDiagnosticScreen.SpawnTrackerLines))]
    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    public static void SpawnTrackerLines(ColonyDiagnosticScreen __instance, int world) {
        __instance.AddDiagnostic<MultiplayerColonyDiagnostic>(
            world,
            __instance.contentContainer,
            __instance.diagnosticRows
        );
    }

}
