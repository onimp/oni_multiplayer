using System;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Players.Events;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Players.Commands;

[Serializable]
public class UpdatePlayerCursorPosition : MultiplayerCommand {

    private PlayerIdentity playerId;
    private Vector2 position;

    public UpdatePlayerCursorPosition(PlayerIdentity playerId, Vector2 position) {
        this.playerId = playerId;
        this.position = position;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var player = context.Multiplayer.Players[playerId];
        context.EventDispatcher.Dispatch(new PlayerCursorPositionUpdatedEvent(player, position));
    }

}
