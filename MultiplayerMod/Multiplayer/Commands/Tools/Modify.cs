using System;
using MultiplayerMod.Game.Context;
using MultiplayerMod.Game.Events.Tools;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Modify : IMultiplayerCommand {

    private ModifyEventArgs @event;

    public Modify(ModifyEventArgs @event) {
        this.@event = @event;
    }

    // ReSharper disable once Unity.IncorrectMonoBehaviourInstantiation
    public void Execute() {
        var tool = new DebugTool {
            type = @event.Type
        };
        GameContextManager.Override(
            new OverrideContext { ModifyParameters = @event.Parameters },
            () => { @event.DragEvent.Cells.ForEach(it => tool.OnDragTool(it, 0)); }
        );
    }

}
