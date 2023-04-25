using System;
using MultiplayerMod.Core.Logging;
using UnityEngine;

namespace MultiplayerMod.Game.Tools.Events;

public static class CopySettingsEvents {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger(typeof(CopySettingsEvents));

    public static event EventHandler<CopySettingsEventArgs> Copy;

    static CopySettingsEvents() => DragToolEvents.DragComplete += OnDragComplete;

    private static void OnDragComplete(object sender, DragCompleteEventArgs e) {
        if (sender is not CopySettingsTool)
            return;

        var lastSelection = GameState.LastSelectedObject;
        var cell = Grid.PosToCell(lastSelection.GetComponent<Transform>().GetPosition());

        var component = lastSelection.GetComponent<BuildingComplete>();
        if (component == null) {
            log.Warning($"Component 'BuildingComplete' not found in {lastSelection.GetProperName()} at cell #{cell}");
            return;
        }

        var layer = component.Def.ObjectLayer;

        Copy?.Invoke(sender, new CopySettingsEventArgs { DragEvent = e, SourceCell = cell, SourceLayer = layer });
    }

}
