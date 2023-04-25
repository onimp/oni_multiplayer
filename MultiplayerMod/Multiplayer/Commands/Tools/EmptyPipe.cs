using System;
using MultiplayerMod.Game.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class EmptyPipe : AbstractDragToolCommand<EmptyPipeTool> {
    public EmptyPipe(DragCompleteEventArgs arguments) : base(arguments) { }
}
