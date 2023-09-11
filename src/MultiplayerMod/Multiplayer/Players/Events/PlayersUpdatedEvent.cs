using MultiplayerMod.Core.Events;

namespace MultiplayerMod.Multiplayer.Players.Events;

public record PlayersUpdatedEvent(MultiplayerPlayers Players) : IDispatchableEvent;
