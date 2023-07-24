using System;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.State;

[Serializable]
public class PlayerState {
    public IPlayer Player { get; }
    public Vector2 CursorPosition { get; set; }
    public bool Spawned { get; set; }

    public PlayerState(IPlayer player) {
        Player = player;
    }
}
