using System;

namespace MultiplayerMod.Network.Command;

public interface ISerializedCommand : IDisposable {
    IntPtr GetPointer();
    long GetSize();
}
