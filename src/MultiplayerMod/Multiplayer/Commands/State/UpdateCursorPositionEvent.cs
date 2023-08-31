using System;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.State;

[Serializable]
public record UpdateCursorPositionEvent(IPlayerIdentity Player, Vector2 Position);
