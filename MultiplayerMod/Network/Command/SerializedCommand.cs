using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace MultiplayerMod.Network.Command;

public class SerializedCommand : ISerializedCommand {

    private readonly MemoryStream memory;
    private GCHandle handle;

    public SerializedCommand(ICommand command) {
        memory = new MemoryStream();
        var formatter = new BinaryFormatter();
        formatter.Serialize(memory, command);
        handle = GCHandle.Alloc(memory.GetBuffer(), GCHandleType.Pinned);
    }

    public void Dispose() {
        handle.Free();
        memory.Close();
    }

    public IntPtr GetPointer() => handle.AddrOfPinnedObject();

    public long GetSize() => memory.Length;

}
