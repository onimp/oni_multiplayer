using System;

namespace MultiplayerMod.Network.Messaging;

public class NetworkMessageHandle : INetworkMessageHandle {

    private readonly IntPtr pointer;
    private readonly long size;

    public NetworkMessageHandle(IntPtr pointer, long size) {
        this.pointer = pointer;
        this.size = size;
    }

    public void Dispose() {
        // No disposal required
    }

    public IntPtr GetPointer() => pointer;

    public uint GetSize() => (uint) size;

}
