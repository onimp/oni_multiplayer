using System;

namespace MultiplayerMod.Multiplayer.Players.Events;

[Serializable]
public record PlayersUpdatedEvent(MultiplayerPlayers Players);
