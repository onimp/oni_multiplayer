using System;

namespace MultiplayerMod.Platform.Base.Network.Messaging;

public interface INetworkMessageHandle : IDisposable {
    public IntPtr Pointer { get; }
    public uint Size { get; }
}
