using System;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.State;

[Serializable]
public class PlayerState {
    public IPlayerIdentity Player { get; }
    public Vector2 CursorPosition { get; set; }
    public bool WorldSpawned { get; set; }

    public PlayerState(IPlayerIdentity player) {
        Player = player;
    }
}
