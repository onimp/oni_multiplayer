using System;
using MultiplayerMod.Platform.Base.Network.Messaging;

namespace MultiplayerMod.Platform.Base.Network;

public static class Configuration {

    public static readonly int MaxMessageSize = 524288; // 512 KiB
    public static readonly int MaxFragmentDataSize = GetFragmentDataSize();

    private static int GetFragmentDataSize() {
        using var serialized = NetworkSerializer.Serialize(new NetworkMessageFragment(0, Array.Empty<byte>()));
        return MaxMessageSize - (int) serialized.Size;
    }

}
