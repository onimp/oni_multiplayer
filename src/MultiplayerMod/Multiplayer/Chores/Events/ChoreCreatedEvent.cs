using System;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.Chores.Events;

public record ChoreCreatedEvent(MultiplayerId Id, Type Type, object?[] Arguments) : IDispatchableEvent;
