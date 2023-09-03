using System;
using MultiplayerMod.Network;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.State;

[Serializable]
public class PlayerState {
    public IMultiplayerClientId Player { get; }

    public PlayerCursor Cursor { get; set; } = new(new Vector2());

    public bool WorldSpawned { get; set; }

    public PlayerState(IMultiplayerClientId player) {
        Player = player;
    }
}
