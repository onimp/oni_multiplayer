using System;
using MultiplayerMod.Game.UI.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class EmptyPipe : AbstractDragToolCommand<EmptyPipeTool> {
    public EmptyPipe(DragCompleteEventArgs arguments) : base(arguments) { }
}
