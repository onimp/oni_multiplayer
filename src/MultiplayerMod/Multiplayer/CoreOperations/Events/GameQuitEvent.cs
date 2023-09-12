using MultiplayerMod.Core.Events;

namespace MultiplayerMod.Multiplayer.CoreOperations.Events;

public record GameQuitEvent(MultiplayerGame Multiplayer) : IDispatchableEvent {
    public bool IsHostMode => Multiplayer.Mode == MultiplayerMode.Host;
}
