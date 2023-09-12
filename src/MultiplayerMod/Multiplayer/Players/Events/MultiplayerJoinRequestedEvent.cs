using MultiplayerMod.Core.Events;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Players.Events;

public record MultiplayerJoinRequestedEvent(IMultiplayerEndpoint Endpoint, string HostName) : IDispatchableEvent;
