using Steamworks;

namespace MultiplayerMod.Platform.Steam.Network;

public static class SteamConfiguration {

    private const int DefaultBufferSize = 10485760; // 10 MiB

    public static SteamNetworkingConfigValue_t SendBufferSize(int size = DefaultBufferSize) => new() {
        m_eValue = ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_SendBufferSize,
        m_eDataType = ESteamNetworkingConfigDataType.k_ESteamNetworkingConfig_Int32,
        m_val = new SteamNetworkingConfigValue_t.OptionValue { m_int32 = size }
    };
}
