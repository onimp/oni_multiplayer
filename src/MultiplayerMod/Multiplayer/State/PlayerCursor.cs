using System;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.State;

[Serializable]
public record PlayerCursor(Vector2 Position, long LastUpdate);
