using MultiplayerMod.Core.Events;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Players.Events;

public record PlayerCursorPositionUpdatedEvent(MultiplayerPlayer Player, Vector2 Position) : IDispatchableEvent;
