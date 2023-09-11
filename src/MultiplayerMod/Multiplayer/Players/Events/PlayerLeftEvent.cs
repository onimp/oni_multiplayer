using MultiplayerMod.Core.Events;

namespace MultiplayerMod.Multiplayer.Players.Events;

public record PlayerLeftEvent(MultiplayerPlayer Player, bool Gracefully) : IDispatchableEvent;
