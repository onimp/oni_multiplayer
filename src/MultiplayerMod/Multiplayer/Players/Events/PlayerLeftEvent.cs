using System;

namespace MultiplayerMod.Multiplayer.Players.Events;

[Serializable]
public record PlayerLeftEvent(MultiplayerPlayer Player, bool Gracefully);
