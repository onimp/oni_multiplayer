using System;

namespace MultiplayerMod.Platform.Base.Network.Messaging;

public class NetworkMessageHandle : INetworkMessageHandle {

    public IntPtr Pointer { get; }
    public uint Size { get; }

    public NetworkMessageHandle(IntPtr pointer, uint size) {
        Pointer = pointer;
        Size = size;
    }

    public void Dispose() {
        // No disposal required
    }

}
