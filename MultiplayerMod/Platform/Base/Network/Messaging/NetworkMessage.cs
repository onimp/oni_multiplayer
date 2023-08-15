using System;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Network;

namespace MultiplayerMod.Platform.Base.Network.Messaging;

[Serializable]
public class NetworkMessage : INetworkMessage {

    public IMultiplayerCommand Command { get; }
    public MultiplayerCommandOptions Options { get; }

    public NetworkMessage(IMultiplayerCommand command, MultiplayerCommandOptions options) {
        Command = command;
        Options = options;
    }

}
