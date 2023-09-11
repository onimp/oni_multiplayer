using MultiplayerMod.Core.Events;

namespace MultiplayerMod.Multiplayer.Players.Events;

public record PlayerStateChangedEvent(MultiplayerPlayer Player, PlayerState State) : IDispatchableEvent;
