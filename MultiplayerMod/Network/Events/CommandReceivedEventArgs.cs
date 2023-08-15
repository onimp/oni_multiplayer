using MultiplayerMod.Multiplayer;

namespace MultiplayerMod.Network.Events;

public record CommandReceivedEventArgs(IPlayer? Player, IMultiplayerCommand Command);
