using System;
using MultiplayerMod.Game.Building;
using MultiplayerMod.Game.Events.Tools;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Stamp : IMultiplayerCommand {

    private StampEventArgs args;

    public Stamp(StampEventArgs args) {
        this.args = args;
    }

    // ReSharper disable once Unity.IncorrectMonoBehaviourInstantiation
    public void Execute() {
        var tool = new StampTool {
            stampTemplate = args.Template,
            ready = true,
            selectAffected = false,
            deactivateOnStamp = false
        };
        try {
            StampCompletion.Override = true;
            tool.Stamp(args.Location);
        } finally {
            StampCompletion.Override = false;
        }
    }

}
