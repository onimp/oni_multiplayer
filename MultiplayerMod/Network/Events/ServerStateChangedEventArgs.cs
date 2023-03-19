using System;

namespace MultiplayerMod.Network.Events;

public class ServerStateChangedEventArgs : EventArgs {
    public MultiplayerServerState State { get; }

    public ServerStateChangedEventArgs(MultiplayerServerState state) {
        State = state;
    }
}
