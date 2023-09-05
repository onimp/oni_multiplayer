using System;
using MultiplayerMod.Multiplayer.Players.Events;

namespace MultiplayerMod.Multiplayer.Players.Commands;

[Serializable]
[MultiplayerCommand(Type = MultiplayerCommandType.System)]
public class AddPlayerCommand : MultiplayerCommand {

    private readonly MultiplayerPlayer player;
    private readonly bool current;

    public AddPlayerCommand(MultiplayerPlayer player, bool current) {
        this.player = player;
        this.current = current;
    }

    public override void Execute(MultiplayerCommandContext context) {
        context.Multiplayer.Players.Add(player);
        if (current)
            context.Multiplayer.Players.SetCurrentPlayerId(player.Id);
        context.EventDispatcher.Dispatch(new PlayerJoinedEvent(player));
    }

}
