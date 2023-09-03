using System;
using MultiplayerMod.Multiplayer;

namespace MultiplayerMod.Network;

public interface IMultiplayerClient {
    MultiplayerClientState State { get; }
    IMultiplayerClientId Id { get; }

    void Connect(IMultiplayerEndpoint endpoint);
    void Disconnect();

    void Send(IMultiplayerCommand command, MultiplayerCommandOptions options = MultiplayerCommandOptions.None);

    event Action<MultiplayerClientState> StateChanged;
    event Action<IMultiplayerCommand> CommandReceived;
}
