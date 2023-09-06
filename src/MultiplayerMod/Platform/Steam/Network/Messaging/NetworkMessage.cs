using System;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Network;

namespace MultiplayerMod.Platform.Steam.Network.Messaging;

[Serializable]
public class NetworkMessage : INetworkMessage {

    public IMultiplayerCommand Command { get; }
    public MultiplayerCommandOptions Options { get; }

    public NetworkMessage(IMultiplayerCommand command, MultiplayerCommandOptions options) {
        Command = command;
        Options = options;
    }

}
