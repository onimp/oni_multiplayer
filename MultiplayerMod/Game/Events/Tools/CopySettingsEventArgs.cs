using System;

namespace MultiplayerMod.Game.Events.Tools;

[Serializable]
public class CopySettingsEventArgs {
    public DragCompleteEventArgs DragEvent { get; set; }
    public int SourceCell { get; set; }
    public ObjectLayer SourceLayer { get; set; }
}
