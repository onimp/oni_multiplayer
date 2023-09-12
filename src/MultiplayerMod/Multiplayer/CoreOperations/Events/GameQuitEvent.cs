using MultiplayerMod.Core.Events;

namespace MultiplayerMod.Multiplayer.CoreOperations.Events;

public record GameQuitEvent(MultiplayerGame Multiplayer) : IDispatchableEvent;
