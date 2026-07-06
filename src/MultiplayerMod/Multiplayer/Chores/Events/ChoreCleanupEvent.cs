using MultiplayerMod.Core.Events;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.Chores.Events;

// Id is the chore's shared MultiplayerId if it was replicated (null for a chore that never had one, e.g. an
// unsupported type). Captured before the object index removes the chore so the host can tell clients which
// replicated chore just ended (see CompleteChore).
public record ChoreCleanupEvent(Chore Chore, MultiplayerId? Id) : IDispatchableEvent;
