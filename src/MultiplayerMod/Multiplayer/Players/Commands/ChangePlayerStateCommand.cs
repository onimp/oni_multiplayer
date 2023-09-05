using System;
using MultiplayerMod.Multiplayer.Players.Events;

namespace MultiplayerMod.Multiplayer.Players.Commands;

[Serializable]
[MultiplayerCommand(Type = MultiplayerCommandType.System)]
public class ChangePlayerStateCommand : MultiplayerCommand {

    private PlayerIdentity playerId;
    private PlayerState state;

    public ChangePlayerStateCommand(PlayerIdentity playerId, PlayerState state) {
        this.playerId = playerId;
        this.state = state;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var player = context.Multiplayer.Players[playerId];
        player.State = state;
        context.EventDispatcher.Dispatch(new PlayerStateChangedEvent(player, state));
    }

}
