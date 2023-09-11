using MultiplayerMod.Core.Events;

namespace MultiplayerMod.Multiplayer.Players.Events;

public record PlayerJoinedEvent(MultiplayerPlayer Player) : IDispatchableEvent;
