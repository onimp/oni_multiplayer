using System;
using MultiplayerMod.Multiplayer;

namespace MultiplayerMod.Network.Messaging;

[Serializable]
public class NetworkMessage {

    public IMultiplayerCommand Command { get; }
    public MultiplayerCommandOptions Options { get; }

    public NetworkMessage(IMultiplayerCommand command, MultiplayerCommandOptions options) {
        Command = command;
        Options = options;
    }

}
