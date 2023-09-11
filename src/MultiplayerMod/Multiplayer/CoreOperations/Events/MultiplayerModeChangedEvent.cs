using MultiplayerMod.Core.Events;

namespace MultiplayerMod.Multiplayer.CoreOperations.Events;

public record MultiplayerModeChangedEvent(MultiplayerMode Mode) : IDispatchableEvent;
