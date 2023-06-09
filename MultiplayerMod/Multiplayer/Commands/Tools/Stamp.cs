using System;
using MultiplayerMod.Game.Context;
using MultiplayerMod.Game.UI.Tools.Context;
using MultiplayerMod.Game.UI.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Stamp : IMultiplayerCommand {

    private StampEventArgs arguments;

    public Stamp(StampEventArgs arguments) {
        this.arguments = arguments;
    }

    // ReSharper disable once Unity.IncorrectMonoBehaviourInstantiation
    public void Execute() {
        var tool = new StampTool {
            stampTemplate = arguments.Template,
            ready = true,
            selectAffected = false,
            deactivateOnStamp = false
        };
        GameContext.Override(new StampCompletionOverride(), () => tool.Stamp(arguments.Location));
    }

}
