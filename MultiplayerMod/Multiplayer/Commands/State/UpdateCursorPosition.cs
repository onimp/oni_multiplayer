using System;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer.State;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.State;

[Serializable]
public class UpdateCursorPosition : MultiplayerCommand {

    private IPlayerIdentity player;
    private Vector2 position;

    public UpdateCursorPosition(IPlayerIdentity player, Vector2 position) {
        this.player = player;
        this.position = position;
    }

    public override void Execute() {
        Dependencies.Get<MultiplayerGame>().State.Players.TryGetValue(player, out var state);
        if (state == null)
            return;

        state.Cursor = new PlayerCursor(position, System.DateTime.Now.Ticks);
    }

}
