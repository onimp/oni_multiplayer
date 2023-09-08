using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer;

public record MultiplayerJoinRequestedEvent(IMultiplayerEndpoint Endpoint, string Name);
