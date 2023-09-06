using System;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Players.Events;

namespace MultiplayerMod.Multiplayer.Players.Commands;

[Serializable]
[MultiplayerCommand(Type = MultiplayerCommandType.System)]
public class RemovePlayerCommand : MultiplayerCommand {

    private PlayerIdentity playerId;

    public RemovePlayerCommand(PlayerIdentity playerId) {
        this.playerId = playerId;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var player = context.Multiplayer.Players[playerId];
        context.Multiplayer.Players.Remove(playerId);
        context.EventDispatcher.Dispatch(new PlayerLeftEvent(player, player.State == PlayerState.Leaving));
    }

}
