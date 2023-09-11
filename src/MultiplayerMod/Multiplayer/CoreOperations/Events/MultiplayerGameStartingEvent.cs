using MultiplayerMod.Core.Events;

namespace MultiplayerMod.Multiplayer.CoreOperations.Events;

public record MultiplayerGameStartingEvent(MultiplayerGame Multiplayer) : IDispatchableEvent;
