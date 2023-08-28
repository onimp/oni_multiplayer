using System;

namespace MultiplayerMod.Platform.Steam.Network.Messaging;

public interface INetworkMessageHandle : IDisposable {
    public IntPtr Pointer { get; }
    public uint Size { get; }
}
