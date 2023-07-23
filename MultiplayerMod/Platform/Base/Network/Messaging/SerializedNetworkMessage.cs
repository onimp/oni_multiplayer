using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using MultiplayerMod.Platform.Base.Network.Messaging.Surrogates;

namespace MultiplayerMod.Platform.Base.Network.Messaging;

public class SerializedNetworkMessage : INetworkMessageHandle {

    public IntPtr Pointer { get; }
    public uint Size { get; }

    private readonly MemoryStream memory;
    private GCHandle handle;

    public SerializedNetworkMessage(INetworkMessage message) {
        memory = new MemoryStream();
        var formatter = new BinaryFormatter { SurrogateSelector = SerializationSurrogates.Selector };
        formatter.Serialize(memory, message);
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
