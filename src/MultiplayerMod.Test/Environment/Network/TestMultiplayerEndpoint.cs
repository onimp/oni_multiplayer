using MultiplayerMod.Network;

namespace MultiplayerMod.Test.Environment.Network;

public record TestMultiplayerEndpoint(TestMultiplayerServer Server) : IMultiplayerEndpoint;
