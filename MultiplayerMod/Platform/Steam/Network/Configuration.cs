using System;
using MultiplayerMod.Platform.Steam.Network.Messaging;
using Steamworks;

namespace MultiplayerMod.Platform.Steam.Network;

public abstract class Configuration {

    private const int DefaultBufferSize = 10485760; // 10 MiB

    public static SteamNetworkingConfigValue_t SendBufferSize(int size = DefaultBufferSize) => new() {
        m_eValue = ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_SendBufferSize,
        m_eDataType = ESteamNetworkingConfigDataType.k_ESteamNetworkingConfig_Int32,
        m_val = new SteamNetworkingConfigValue_t.OptionValue { m_int32 = size }
    };

    public static readonly int MaxMessageSize = 524288; // 512 KiB
    public static readonly int MaxFragmentDataSize = GetFragmentDataSize();

    private static int GetFragmentDataSize() {
        using var serialized = NetworkSerializer.Serialize(new NetworkMessageFragment(0, Array.Empty<byte>()));
        return MaxMessageSize - (int) serialized.Size;
    }

}
