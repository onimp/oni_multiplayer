using System;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.State;

[Serializable]
public class PlayerState {
    public IPlayerIdentity Player { get; }

    public Vector2 PrevCursorLocation;
    public Vector2 NewCursorLocation;
    public float PrevCursorTime;
    public float NewCursorTime;

    public bool WorldSpawned { get; set; }

    public PlayerState(IPlayerIdentity player) {
        Player = player;
    }
}
