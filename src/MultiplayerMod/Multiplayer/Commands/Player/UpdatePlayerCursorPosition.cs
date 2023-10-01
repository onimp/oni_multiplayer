using System;
using MultiplayerMod.Game.UI.Tools.Events;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Multiplayer.Players.Events;

namespace MultiplayerMod.Multiplayer.Commands.Player;

[Serializable]
public class UpdatePlayerCursorPosition : MultiplayerCommand {

    private PlayerIdentity playerId;
    private InterfaceToolEvents.MouseMovedEventArgs mouseMovedEventArgs;

    public UpdatePlayerCursorPosition(
        PlayerIdentity playerId,
        InterfaceToolEvents.MouseMovedEventArgs mouseMovedEventArgs
    ) {
        this.playerId = playerId;
        this.mouseMovedEventArgs = mouseMovedEventArgs;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var player = context.Multiplayer.Players[playerId];
        context.EventDispatcher.Dispatch(new PlayerCursorPositionUpdatedEvent(player, mouseMovedEventArgs));
    }

}
