using MultiplayerMod.Core.Events;

namespace MultiplayerMod.Multiplayer.CoreOperations.Events;

public record StopMultiplayerEvent(MultiplayerGame Multiplayer) : IDispatchableEvent;
