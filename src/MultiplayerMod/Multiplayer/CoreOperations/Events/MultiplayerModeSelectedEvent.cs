using MultiplayerMod.Core.Events;

namespace MultiplayerMod.Multiplayer.CoreOperations.Events;

public record MultiplayerModeSelectedEvent(MultiplayerMode Mode) : IDispatchableEvent;
