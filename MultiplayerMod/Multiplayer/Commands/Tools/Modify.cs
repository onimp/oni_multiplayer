using System;
using MultiplayerMod.Game.Context;
using MultiplayerMod.Game.UI.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Modify : MultiplayerCommand {

    private ModifyEventArgs arguments;

    public Modify(ModifyEventArgs arguments) {
        this.arguments = arguments;
    }

    // ReSharper disable once Unity.IncorrectMonoBehaviourInstantiation
    public override void Execute() {
        var tool = new DebugTool {
            type = arguments.Type
        };
        GameContext.Override(
            arguments.ToolContext,
            () => { arguments.DragEventArgs.Cells.ForEach(it => tool.OnDragTool(it, 0)); }
        );
    }

}
