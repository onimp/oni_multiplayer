using System;
using MultiplayerMod.Game.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Disconnect : AbstractDragToolCommand<DisconnectTool> {

    public Disconnect(DragCompleteEventArgs arguments) : base(arguments) { }

    protected override void InvokeTool(DisconnectTool tool) {
        tool.OnDragComplete(Arguments.CursorDown, Arguments.CursorUp);
    }

}
