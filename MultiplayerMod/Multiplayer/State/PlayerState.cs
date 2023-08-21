using System;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.State;

[Serializable]
public class PlayerState {
    public IPlayerIdentity Player { get; }

    public PlayerCursor Cursor { get; set; } = new(new Vector2(), System.DateTime.Now.Ticks);

    public bool WorldSpawned { get; set; }

    public PlayerState(IPlayerIdentity player) {
        Player = player;
    }
}
