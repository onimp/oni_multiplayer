using MultiplayerMod.Multiplayer;

namespace MultiplayerMod.Network.Events;

public record CommandReceivedEventArgs(IPlayerIdentity? Player, IMultiplayerCommand Command);
