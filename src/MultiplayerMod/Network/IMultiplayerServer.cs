using System;
using System.Collections.Generic;
using MultiplayerMod.Multiplayer;

namespace MultiplayerMod.Network;

public interface IMultiplayerServer {
    void Start();
    void Stop();

    MultiplayerServerState State { get; }
    IMultiplayerEndpoint Endpoint { get; }
    List<IMultiplayerClientId> Clients { get; }

    void Send(IMultiplayerClientId clientId, IMultiplayerCommand command);
    void Send(IMultiplayerCommand command, MultiplayerCommandOptions options = MultiplayerCommandOptions.None);

    event Action<MultiplayerServerState> StateChanged;
    event Action<IMultiplayerClientId> ClientConnected;
    event Action<IMultiplayerClientId> ClientDisconnected;
    event Action<IMultiplayerClientId, IMultiplayerCommand> CommandReceived;
}
