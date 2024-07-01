using MultiplayerMod.Core.Events;

namespace MultiplayerMod.Multiplayer.Chores.Events;

public record ChoreCleanupEvent(Chore Chore) : IDispatchableEvent;
