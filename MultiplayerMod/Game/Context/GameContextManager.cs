namespace MultiplayerMod.Game.Context;

public abstract class GameContextManager {

    private static readonly GameContextOverride @override = new();

    public static void Override(OverrideContext context, System.Action action) {
        var originalContext = GameContext.GetCurrent();
        try {
            @override.Restore();
            OverridePriority(context);
            OverrideModifyParameters(context);
            action();
        } finally {
            originalContext.Restore();
        }
    }

    private static void OverridePriority(OverrideContext context) {
        if (context.Priority == null)
            return;

        @override.PriorityScreen.lastSelectedPriority = context.Priority.Value;
    }

    private static void OverrideModifyParameters(OverrideContext context) {
        if (context.ModifyParameters == null)
            return;

        if (context.ModifyParameters.Element != null) {
            @override.DebugPaintElementScreen.paintElement.isOn = true;
            @override.DebugPaintElementScreen.element = context.ModifyParameters.Element.Value;
        }
        if (context.ModifyParameters.Temperature != null) {
            @override.DebugPaintElementScreen.paintTemperature.isOn = true;
            @override.DebugPaintElementScreen.temperature = context.ModifyParameters.Temperature.Value;
        }
        if (context.ModifyParameters.Mass != null) {
            @override.DebugPaintElementScreen.paintMass.isOn = true;
            @override.DebugPaintElementScreen.mass = context.ModifyParameters.Mass.Value;
        }
        if (context.ModifyParameters.DiseaseCount != null) {
            @override.DebugPaintElementScreen.paintDiseaseCount.isOn = true;
            @override.DebugPaintElementScreen.diseaseCount = context.ModifyParameters.DiseaseCount.Value;
        }
        if (context.ModifyParameters.DiseaseType != null) {
            @override.DebugPaintElementScreen.paintDisease.isOn = true;
            @override.DebugPaintElementScreen.diseaseIdx = context.ModifyParameters.DiseaseType.Value;
        }

        @override.DebugPaintElementScreen.affectBuildings.isOn = context.ModifyParameters.AffectBuildings;
        @override.DebugPaintElementScreen.affectCells.isOn = context.ModifyParameters.AffectCells;
        @override.DebugPaintElementScreen.set_prevent_fow_reveal = context.ModifyParameters.PreventFowReveal;
        @override.DebugPaintElementScreen.set_allow_fow_reveal = context.ModifyParameters.AllowFowReveal;
    }

}
