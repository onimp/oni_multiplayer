using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MultiplayerMod.Platform.Steam.Network.Messaging;

public class SerializedNetworkMessage : INetworkMessageHandle {

    public IntPtr Pointer { get; }
    public uint Size { get; }

    private readonly MemoryStream memory;
    private GCHandle handle;

    public SerializedNetworkMessage(INetworkMessage message) {
        memory = new MemoryStream();
        NetworkSerializer.CreateFormatter().Serialize(memory, message);
        handle = GCHandle.Alloc(memory.GetBuffer(), GCHandleType.Pinned);
        Pointer = handle.AddrOfPinnedObject();
        Size = (uint) memory.Length;
    }

    public void Dispose() {
        handle.Free();
        memory.Close();
    }

    public byte[] GetBuffer() => memory.GetBuffer();

}
