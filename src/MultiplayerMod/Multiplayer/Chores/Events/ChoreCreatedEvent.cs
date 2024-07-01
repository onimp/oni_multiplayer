using System;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.Chores.Events;

public record ChoreCreatedEvent(Chore Chore, MultiplayerId Id, Type Type, object?[] Arguments) : IDispatchableEvent;
