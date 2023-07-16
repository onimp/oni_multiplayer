using System;

namespace MultiplayerMod.Game.UI.Tools.Events;

[Serializable]
public class CopySettingsEventArgs {
    public DragCompleteEventArgs DragEvent { get; }
    public int SourceCell { get; }
    public ObjectLayer SourceLayer { get; }

    public CopySettingsEventArgs(DragCompleteEventArgs dragEvent, int sourceCell, ObjectLayer sourceLayer) {
        DragEvent = dragEvent;
        SourceCell = sourceCell;
        SourceLayer = sourceLayer;
    }
}
