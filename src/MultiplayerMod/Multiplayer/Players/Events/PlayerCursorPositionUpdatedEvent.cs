using System;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Players.Events;

[Serializable]
public record PlayerCursorPositionUpdatedEvent(MultiplayerPlayer Player, Vector2 Position);
