using System;
using System.Collections.Generic;
using MultiplayerMod.Game.UI.Tools.Context;
using static DebugTool.Type;

namespace MultiplayerMod.Game.UI.Tools.Events;

public static class DebugToolEvents {

    private static readonly HashSet<DebugTool.Type> modificationTypes = new() {
        ReplaceSubstance,
        FillReplaceSubstance,
        Clear,
        Deconstruct,
        Destroy,
        StoreSubstance
    };

    public static event EventHandler<ModifyEventArgs>? Modify;

    static DebugToolEvents() => DragToolEvents.DragComplete += OnDragComplete;

    private static void OnDragComplete(object sender, DragCompleteEventArgs e) {
        if (sender is not DebugTool tool)
            return;

        if (!modificationTypes.Contains(tool.type))
            return;

        var instance = DebugPaintElementScreen.Instance;
        var context = new DebugToolContext {
            Element = instance.paintElement.isOn ? instance.element : null,
            DiseaseType = instance.paintDisease.isOn ? instance.diseaseIdx : null,
            DiseaseCount = instance.paintDiseaseCount.isOn ? instance.diseaseCount : null,
            Temperature = instance.paintTemperature.isOn ? instance.temperature : null,
            Mass = instance.paintMass.isOn ? instance.mass : null,
            AffectCells = instance.affectCells.isOn,
            AffectBuildings = instance.affectBuildings.isOn,
            PreventFowReveal = instance.set_prevent_fow_reveal,
            AllowFowReveal = instance.set_allow_fow_reveal
        };

        Modify?.Invoke(sender, new ModifyEventArgs(e, tool.type, context));
    }

}
