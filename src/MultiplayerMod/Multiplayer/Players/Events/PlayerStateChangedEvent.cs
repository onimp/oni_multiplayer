using System;

namespace MultiplayerMod.Multiplayer.Players.Events;

[Serializable]
public record PlayerStateChangedEvent(MultiplayerPlayer Player, PlayerState State);
