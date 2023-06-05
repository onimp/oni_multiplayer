using System;
using MultiplayerMod.Game.UI.Tools.Context;

namespace MultiplayerMod.Game.UI.Tools.Events;

[Serializable]
public class ModifyEventArgs {
    public DragCompleteEventArgs DragEventArgs { get; set; }
    public DebugTool.Type Type { get; set; }
    public DebugToolContext ToolContext { get; set; }
}
