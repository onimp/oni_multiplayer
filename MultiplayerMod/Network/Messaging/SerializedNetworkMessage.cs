using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace MultiplayerMod.Network.Messaging;

public class SerializedNetworkMessage : INetworkMessageHandle {

    private readonly MemoryStream memory;
    private GCHandle handle;

    public SerializedNetworkMessage(NetworkMessage message) {
        memory = new MemoryStream();
        var formatter = new BinaryFormatter();
        formatter.Serialize(memory, message);
        handle = GCHandle.Alloc(memory.GetBuffer(), GCHandleType.Pinned);
    }

    public void Dispose() {
        handle.Free();
        memory.Close();
    }

    public IntPtr GetPointer() => handle.AddrOfPinnedObject();

    public uint GetSize() => (uint) memory.Length;

}
