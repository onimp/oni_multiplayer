using System;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Network;
using MultiplayerMod.Platform.Steam.Network.Messaging;
using Steamworks;
using static Steamworks.Constants;

namespace MultiplayerMod.Platform.Steam.Network;

public static class Configuration {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger(typeof(Configuration));

    private const int defaultBufferSize = 10485760; // 10 MiB
    private const int defaultSendRateMax = 8388608; // 8 MiB/s

    public static SteamNetworkingConfigValue_t SendBufferSize(int size = defaultBufferSize) => new() {
        m_eValue = ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_SendBufferSize,
        m_eDataType = ESteamNetworkingConfigDataType.k_ESteamNetworkingConfig_Int32,
        m_val = new SteamNetworkingConfigValue_t.OptionValue { m_int32 = size }
    };

    // Raise only the upper bound of GameNetworkingSockets' send pacing. Its bandwidth estimator still ramps
    // up from SendRateMin and backs off on loss, so this never forces traffic onto a bad link — it just
    // stops a good link (e.g. LAN) from being needlessly ceilinged while the multi-MB hard-sync streams.
    // (RecvBufferSize would be the more direct knob, but it postdates the Steam SDK ONI ships and isn't in
    // this Steamworks.NET binding, so it can't be set here.)
    public static SteamNetworkingConfigValue_t SendRateMax(int bytesPerSec = defaultSendRateMax) => new() {
        m_eValue = ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_SendRateMax,
        m_eDataType = ESteamNetworkingConfigDataType.k_ESteamNetworkingConfig_Int32,
        m_val = new SteamNetworkingConfigValue_t.OptionValue { m_int32 = bytesPerSec }
    };

    public const int MaxMessageSize = 524288; // 512 KiB
    public static readonly int MaxFragmentDataSize = GetFragmentDataSize();

    private static int GetFragmentDataSize() {
        using var serialized = NetworkSerializer.Serialize(new NetworkMessageFragment(0, Array.Empty<byte>()));
        return MaxMessageSize - (int) serialized.Size;
    }

    // --- Priority lanes -----------------------------------------------------------------------------
    // One lane per NetworkLane value, in enum order. Lower priority number = sent first, so the
    // Gameplay lane always drains ahead of Sim/Bulk. Equal weights (irrelevant while priorities differ).
    public const int LaneCount = 3;
    private static readonly int[] lanePriorities = { 0, 10, 20 };
    private static readonly ushort[] laneWeights = { 1, 1, 1 };

    /// <summary>Steam send flags for a lane: the Sim lane is unreliable (idempotent, re-sent), the rest reliable.</summary>
    public static int SendFlags(NetworkLane lane) =>
        lane == NetworkLane.Sim ? k_nSteamNetworkingSend_Unreliable : k_nSteamNetworkingSend_Reliable;

    /// <summary>Configure the priority lanes on a client (user) connection. Returns true on success.</summary>
    public static bool ConfigureClientLanes(HSteamNetConnection connection) {
        // pLanePriorities / pLaneWeights are declared `out` in Steamworks.NET but the native call reads
        // them as input arrays; because they're blittable-by-ref the pinned address is passed through, so
        // native still sees our contents. Pass the first element of each contiguous array.
        var result = SteamNetworkingSockets.ConfigureConnectionLanes(
            connection, LaneCount, out lanePriorities[0], out laneWeights[0]
        );
        return LogLaneResult(result);
    }

    /// <summary>Configure the priority lanes on a game-server connection. Returns true on success.</summary>
    public static bool ConfigureServerLanes(HSteamNetConnection connection) {
        var result = SteamGameServerNetworkingSockets.ConfigureConnectionLanes(
            connection, LaneCount, out lanePriorities[0], out laneWeights[0]
        );
        return LogLaneResult(result);
    }

    private static bool LogLaneResult(EResult result) {
        if (result == EResult.k_EResultOK)
            return true;
        log.Warning($"ConfigureConnectionLanes failed ({result}); falling back to single-lane sends");
        return false;
    }

}
