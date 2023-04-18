using System;
using System.Collections.Generic;
using MultiplayerMod.Game.Debug;
using static DebugTool.Type;

namespace MultiplayerMod.Game.Events.Tools;

public static class DebugToolEvents {

    private static readonly HashSet<DebugTool.Type> ModificationTypes = new() {
        ReplaceSubstance,
        FillReplaceSubstance,
        Clear,
        Deconstruct,
        Destroy,
        StoreSubstance
    };

    public static event EventHandler<ModifyEventArgs> Modify;

    static DebugToolEvents() => DragToolEvents.DragComplete += OnDragComplete;

    private static void OnDragComplete(object sender, DragCompleteEventArgs e) {
        if (sender is not DebugTool tool)
            return;

        if (!ModificationTypes.Contains(tool.type))
            return;

        var parameters = new ModifyParameters {
            AffectCells = DebugPaintElementScreen.Instance.affectCells.isOn,
            AffectBuildings = DebugPaintElementScreen.Instance.affectBuildings.isOn,
            PreventFowReveal = DebugPaintElementScreen.Instance.set_prevent_fow_reveal,
            AllowFowReveal = DebugPaintElementScreen.Instance.set_allow_fow_reveal
        };

        if (DebugPaintElementScreen.Instance.paintElement.isOn)
            parameters.Element = DebugPaintElementScreen.Instance.element;
        if (DebugPaintElementScreen.Instance.paintDisease.isOn)
            parameters.DiseaseType = DebugPaintElementScreen.Instance.diseaseIdx;
        if (DebugPaintElementScreen.Instance.paintDiseaseCount.isOn)
            parameters.DiseaseCount = DebugPaintElementScreen.Instance.diseaseCount;
        if (DebugPaintElementScreen.Instance.paintTemperature.isOn)
            parameters.Temperature = DebugPaintElementScreen.Instance.temperature;
        if (DebugPaintElementScreen.Instance.paintMass.isOn)
            parameters.Mass = DebugPaintElementScreen.Instance.mass;

        Modify?.Invoke(sender, new ModifyEventArgs { DragEvent = e, Type = tool.type, Parameters = parameters });
    }

}
