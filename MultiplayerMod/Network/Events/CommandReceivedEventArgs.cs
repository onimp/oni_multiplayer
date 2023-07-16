using MultiplayerMod.Multiplayer;

namespace MultiplayerMod.Network.Events;

public class CommandReceivedEventArgs {
    public IPlayer? Player { get; }
    public IMultiplayerCommand Command { get; }

    public CommandReceivedEventArgs(IPlayer? player, IMultiplayerCommand command) {
        Player = player;
        Command = command;
    }
}
