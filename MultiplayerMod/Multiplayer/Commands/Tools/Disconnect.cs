using System;
using MultiplayerMod.Game.Events.Tools;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Disconnect : AbstractDragToolCommand<DisconnectTool> {

    public Disconnect(DragCompleteEventArgs @event) : base(@event) { }

    protected override void InvokeTool(DisconnectTool tool) {
        tool.OnDragComplete(Event.CursorDown, Event.CursorUp);
    }

}
