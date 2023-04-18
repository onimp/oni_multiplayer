using System;
using MultiplayerMod.Game.Debug;

namespace MultiplayerMod.Game.Events.Tools;

[Serializable]
public class ModifyEventArgs {
    public DragCompleteEventArgs DragEvent { get; set; }
    public DebugTool.Type Type { get; set; }
    public ModifyParameters Parameters { get; set; }
}
