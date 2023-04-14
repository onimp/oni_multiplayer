using System;
using MultiplayerMod.Game.Events.Tools;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Cancel : AbstractDragToolCommand<CancelTool> {
    public Cancel(DragCompleteEventArgs @event) : base(@event) { }
}
