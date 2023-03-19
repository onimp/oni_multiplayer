using System;

namespace MultiplayerMod.Network.Messaging;

public interface INetworkMessageHandle : IDisposable {
    IntPtr GetPointer();
    uint GetSize();
}
