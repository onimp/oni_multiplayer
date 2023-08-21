using System;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.State;

[Serializable]
public class PlayerState {
    public IPlayerIdentity Player { get; }

    public Vector2 CursorPosition {
        get {
            var updateDelta = newCursorTime - prevCursorTime;
            var timeDiff = (Time.time - newCursorTime) / updateDelta;
            return Vector2.Lerp(prevCursorLocation, newCursorLocation, timeDiff);
        }

        set {
            prevCursorLocation = newCursorLocation;
            prevCursorTime = newCursorTime;
            newCursorLocation = value;
            newCursorTime = Time.time;
        }
    }

    public bool WorldSpawned { get; set; }

    private Vector2 prevCursorLocation;
    private Vector2 newCursorLocation;
    private float prevCursorTime;
    private float newCursorTime;

    public PlayerState(IPlayerIdentity player) {
        Player = player;
    }
}
