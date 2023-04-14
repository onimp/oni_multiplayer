using System;
using MultiplayerMod.Game.Events.Tools;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class EmptyPipe : AbstractDragToolCommand<EmptyPipeTool> {
    public EmptyPipe(DragCompleteEventArgs @event) : base(@event) { }
}
