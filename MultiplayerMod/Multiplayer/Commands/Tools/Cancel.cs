using System;
using MultiplayerMod.Game.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Cancel : AbstractDragToolCommand<CancelTool> {
    public Cancel(DragCompleteEventArgs arguments) : base(arguments) { }
}
