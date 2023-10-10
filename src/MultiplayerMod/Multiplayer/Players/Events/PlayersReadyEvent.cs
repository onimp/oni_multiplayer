using MultiplayerMod.Core.Events;

namespace MultiplayerMod.Multiplayer.Players.Events;

public record PlayersReadyEvent(MultiplayerGame Multiplayer) : IDispatchableEvent;
