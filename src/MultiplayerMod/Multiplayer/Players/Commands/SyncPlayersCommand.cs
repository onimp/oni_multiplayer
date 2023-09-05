using System;
using MultiplayerMod.Multiplayer.Players.Events;

namespace MultiplayerMod.Multiplayer.Players.Commands;

[Serializable]
[MultiplayerCommand(Type = MultiplayerCommandType.System)]
public class SyncPlayersCommand : MultiplayerCommand {

    private MultiplayerPlayer[] players;

    public SyncPlayersCommand(MultiplayerPlayer[] players) {
        this.players = players;
    }

    public override void Execute(MultiplayerCommandContext context) {
        context.Multiplayer.Players.Synchronize(players);
        context.EventDispatcher.Dispatch(new PlayersUpdatedEvent(context.Multiplayer.Players));
    }

}
