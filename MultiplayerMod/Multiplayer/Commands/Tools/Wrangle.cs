using System;
using MultiplayerMod.Game.Events.Tools;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Wrangle : AbstractDragToolCommand<CaptureTool> {
    public Wrangle(DragCompleteEventArgs @event) : base(@event) { }
    protected override void InvokeTool(CaptureTool tool) => tool.OnDragComplete(Event.CursorDown, Event.CursorUp);
}
