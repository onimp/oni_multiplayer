using System;
using MultiplayerMod.Game.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Wrangle : AbstractDragToolCommand<CaptureTool> {
    public Wrangle(DragCompleteEventArgs arguments) : base(arguments) { }

    protected override void InvokeTool(CaptureTool tool) =>
        tool.OnDragComplete(Arguments.CursorDown, Arguments.CursorUp);
}
