using MultiplayerMod.Core.Events;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Players.Events;

public record MultiplayerConnectRequestedEvent(IMultiplayerEndpoint Endpoint, string Name) : IDispatchableEvent;
