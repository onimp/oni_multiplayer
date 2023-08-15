using System;
using MultiplayerMod.Multiplayer.State;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.State;

[Serializable]
public class UpdateCursorPosition : MultiplayerCommand {

    private IPlayer player;
    private Vector2 position;

    public UpdateCursorPosition(IPlayer player, Vector2 position) {
        this.player = player;
        this.position = position;
    }

    public override void Execute() {
        MultiplayerGame.State.Players.TryGetValue(player, out var state);
        if (state == null)
            return;

        state.CursorPosition = position;
    }

}
