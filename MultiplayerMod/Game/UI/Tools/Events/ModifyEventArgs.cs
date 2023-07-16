using System;
using MultiplayerMod.Game.UI.Tools.Context;

namespace MultiplayerMod.Game.UI.Tools.Events;

[Serializable]
public class ModifyEventArgs {
    public DragCompleteEventArgs DragEventArgs { get; }
    public DebugTool.Type Type { get; }
    public DebugToolContext ToolContext { get; }

    public ModifyEventArgs(DragCompleteEventArgs dragEventArgs, DebugTool.Type type, DebugToolContext toolContext) {
        DragEventArgs = dragEventArgs;
        Type = type;
        ToolContext = toolContext;
    }
}
