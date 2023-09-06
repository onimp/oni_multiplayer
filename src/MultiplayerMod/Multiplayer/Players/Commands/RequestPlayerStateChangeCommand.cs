using System;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Players.Commands;

[Serializable]
[MultiplayerCommand(Type = MultiplayerCommandType.System)]
public class RequestPlayerStateChangeCommand : MultiplayerCommand {

    private PlayerIdentity playerId;
    private PlayerState state;

    public RequestPlayerStateChangeCommand(PlayerIdentity playerId, PlayerState state) {
        this.playerId = playerId;
        this.state = state;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var server = context.Runtime.Dependencies.Get<IMultiplayerServer>();
        server.Send(new ChangePlayerStateCommand(playerId, state));
    }

}
