using MultiplayerMod.Core.Events;

namespace MultiplayerMod.Multiplayer.World.Debug;

public record DebugSnapshotAvailableEvent(WorldDebugSnapshot Snapshot) : IDispatchableEvent;
