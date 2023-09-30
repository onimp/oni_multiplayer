using System;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Multiplayer.Players.Events;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.Player;

[Serializable]
public class UpdatePlayerCursorPosition : MultiplayerCommand {

    private PlayerIdentity playerId;
    private Vector2 position;
    private string? screenName;

    public UpdatePlayerCursorPosition(PlayerIdentity playerId, Vector2 position, string? screenName) {
        this.playerId = playerId;
        this.position = position;
        this.screenName = screenName;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var player = context.Multiplayer.Players[playerId];
        context.EventDispatcher.Dispatch(new PlayerCursorPositionUpdatedEvent(player, position, screenName));
    }

}
