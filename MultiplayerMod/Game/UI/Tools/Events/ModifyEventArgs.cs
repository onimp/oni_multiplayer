using System;
using MultiplayerMod.Game.UI.Tools.Context;

namespace MultiplayerMod.Game.UI.Tools.Events;

[Serializable]
public record ModifyEventArgs(
    DragCompleteEventArgs DragEventArgs,
    DebugTool.Type Type,
    DebugToolContext ToolContext
);
