using System;
using MultiplayerMod.Game.UI.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Attack : AbstractDragToolCommand<AttackTool> {
    public Attack(DragCompleteEventArgs arguments) : base(arguments) { }

    protected override void InvokeTool(AttackTool tool) =>
        tool.OnDragComplete(Arguments.CursorDown, Arguments.CursorUp);
}
