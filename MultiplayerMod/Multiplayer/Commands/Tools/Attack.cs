using System;
using MultiplayerMod.Game.Events.Tools;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Attack : AbstractDragToolCommand<AttackTool> {
    public Attack(DragCompleteEventArgs @event) : base(@event) { }
    protected override void InvokeTool(AttackTool tool) => tool.OnDragComplete(Event.CursorDown, Event.CursorUp);
}
