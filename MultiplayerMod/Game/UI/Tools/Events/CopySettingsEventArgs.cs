using System;

namespace MultiplayerMod.Game.UI.Tools.Events;

[Serializable]
public record CopySettingsEventArgs(
    DragCompleteEventArgs DragEvent,
    int SourceCell,
    ObjectLayer SourceLayer
);
