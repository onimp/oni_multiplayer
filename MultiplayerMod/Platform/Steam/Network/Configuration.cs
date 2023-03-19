using Steamworks;

namespace MultiplayerMod.Platform.Steam.Network;

public abstract class Configuration {

    public static SteamNetworkingConfigValue_t SendBufferSize(int size = 10485760) => new() {
        m_eValue = ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_SendBufferSize,
        m_eDataType = ESteamNetworkingConfigDataType.k_ESteamNetworkingConfig_Int32,
        m_val = new SteamNetworkingConfigValue_t.OptionValue { m_int32 = size }
    };

}
