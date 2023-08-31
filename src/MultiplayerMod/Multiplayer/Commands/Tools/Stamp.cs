using System;
using MultiplayerMod.Game.Context;
using MultiplayerMod.Game.UI.Tools.Context;
using MultiplayerMod.Game.UI.Tools.Events;
using MultiplayerMod.ModRuntime;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Stamp : MultiplayerCommand {

    private StampEventArgs arguments;

    public Stamp(StampEventArgs arguments) {
        this.arguments = arguments;
    }

    // ReSharper disable once Unity.IncorrectMonoBehaviourInstantiation
    public override void Execute(Runtime runtime) {
        var tool = new StampTool {
            stampTemplate = arguments.Template,
            ready = true,
            selectAffected = false,
            deactivateOnStamp = false
        };
        GameContext.Override(new StampCompletionOverride(), () => tool.Stamp(arguments.Location));
    }

}
