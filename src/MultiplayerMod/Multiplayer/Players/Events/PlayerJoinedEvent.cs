using System;

namespace MultiplayerMod.Multiplayer.Players.Events;

[Serializable]
public record PlayerJoinedEvent(MultiplayerPlayer Player);
