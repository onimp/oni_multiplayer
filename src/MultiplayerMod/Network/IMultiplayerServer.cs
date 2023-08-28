using System;
using System.Collections.Generic;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Network.Events;

namespace MultiplayerMod.Network;

public interface IMultiplayerServer {
    void Start();
    void Stop();

    MultiplayerServerState State { get; }
    IMultiplayerEndpoint Endpoint { get; }
    List<IPlayerIdentity> Players { get; }

    void Send(IPlayerIdentity player, IMultiplayerCommand command);
    void Send(IMultiplayerCommand command, MultiplayerCommandOptions options = MultiplayerCommandOptions.None);

    event Action<ServerStateChangedEventArgs> StateChanged;
    event Action<IPlayerIdentity> PlayerConnected;
    event Action<IPlayerIdentity> PlayerDisconnected;
    event Action<CommandReceivedEventArgs> CommandReceived;
}
