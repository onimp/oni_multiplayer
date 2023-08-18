using System;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Network.Events;

namespace MultiplayerMod.Network;

public interface IMultiplayerClient {
    MultiplayerClientState State { get; }
    IPlayerIdentity Player { get; }

    void Connect(IMultiplayerEndpoint endpoint);
    void Disconnect();

    void Send(IMultiplayerCommand command, MultiplayerCommandOptions options = MultiplayerCommandOptions.None);

    event Action<MultiplayerClientState> StateChanged;
    event Action<CommandReceivedEventArgs> CommandReceived;
}
