using System;

namespace MultiplayerMod.Network.Events;

public class ClientStateChangedEventArgs : EventArgs {
    public MultiplayerClientState State { get; }

    public ClientStateChangedEventArgs(MultiplayerClientState state) {
        State = state;
    }
}
