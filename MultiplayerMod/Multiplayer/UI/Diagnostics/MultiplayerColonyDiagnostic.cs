using MultiplayerMod.Multiplayer.UI.WorldTrackers;

namespace MultiplayerMod.Multiplayer.UI.Diagnostics;

public class MultiplayerColonyDiagnostic : ColonyDiagnostic {

    public MultiplayerColonyDiagnostic(int worldId) : base(worldId, "Multiplayer errors") {
        icon = "icon_errand_operate";
        tracker = TrackerTool.Instance.GetWorldTracker<MultiplayerErrorsTracker>(worldId);
        AddCriterion("CheckSyncErrors", new DiagnosticCriterion("Check sync errors", CheckSyncErrorsCount));
    }

    private DiagnosticResult CheckSyncErrorsCount() {
        var diagnosticResult = new DiagnosticResult(
            DiagnosticResult.Opinion.Normal,
            STRINGS.UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS
        );

        var averageValue = tracker.GetAverageValue(5f);

        // TODO adjust levels
        if (averageValue < 10.0) {
            diagnosticResult.opinion = DiagnosticResult.Opinion.Normal;
            diagnosticResult.Message = "No sync errors";
        } else if (averageValue < 100.0) {
            diagnosticResult.opinion = DiagnosticResult.Opinion.Concern;
            diagnosticResult.Message = "Low sync errors";
        } else if (averageValue < 500.0) {
            diagnosticResult.opinion = DiagnosticResult.Opinion.Warning;
            diagnosticResult.Message = "Medium sync errors";
        } else {
            diagnosticResult.opinion = DiagnosticResult.Opinion.Bad;
            diagnosticResult.Message = "High sync errors";
        }

        return diagnosticResult;
    }

}
