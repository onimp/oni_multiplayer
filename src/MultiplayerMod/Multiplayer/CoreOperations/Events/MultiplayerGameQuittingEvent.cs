using MultiplayerMod.Core.Events;

namespace MultiplayerMod.Multiplayer.CoreOperations.Events;

public record MultiplayerGameQuittingEvent(MultiplayerGame Multiplayer) : IDispatchableEvent;
