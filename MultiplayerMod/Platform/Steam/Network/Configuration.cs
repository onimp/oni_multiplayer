using System;
using MultiplayerMod.Platform.Steam.Network.Messaging;
using Steamworks;

namespace MultiplayerMod.Platform.Steam.Network;

public abstract class Configuration {

    public static SteamNetworkingConfigValue_t SendBufferSize(int size = 10485760) => new() {
        m_eValue = ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_SendBufferSize,
        m_eDataType = ESteamNetworkingConfigDataType.k_ESteamNetworkingConfig_Int32,
        m_val = new SteamNetworkingConfigValue_t.OptionValue { m_int32 = size }
    };

    public static readonly int MaxMessageSize = 512 * 1024;
    public static readonly int MaxFragmentDataSize = GetFragmentDataSize();

    private static int GetFragmentDataSize() {
        using var serialized = NetworkSerializer.Serialize(new NetworkMessageFragment(0, Array.Empty<byte>()));
        return MaxMessageSize - (int) serialized.Size;
    }

}
