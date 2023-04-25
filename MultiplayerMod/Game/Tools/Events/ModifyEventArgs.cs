using System;
using MultiplayerMod.Game.Tools.Context;

namespace MultiplayerMod.Game.Tools.Events;

[Serializable]
public class ModifyEventArgs {
    public DragCompleteEventArgs DragEventArgs { get; set; }
    public DebugTool.Type Type { get; set; }
    public DebugToolContext ToolContext { get; set; }
}
