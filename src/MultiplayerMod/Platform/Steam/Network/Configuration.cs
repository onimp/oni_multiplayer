using System;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Network;
using MultiplayerMod.Platform.Steam.Network.Messaging;
using Steamworks;
using static Steamworks.Constants;

namespace MultiplayerMod.Platform.Steam.Network;

public static class Configuration {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger(typeof(Configuration));

    // Steam's per-connection reliable send buffer. This must comfortably exceed the largest single burst
    // the host ever enqueues in one go, because SendMessageToConnection rejects (k_EResultLimitExceeded)
    // and DROPS any message that would overflow it. The hard-sync enqueues the whole save as a header +
    // N fragments synchronously, ON TOP of whatever gameplay backlog is already buffered — so at the old
    // 10 MiB a big save (or a modest save behind a few MB of backlog) silently lost its tail fragments,
    // leaving the client's reassembly permanently incomplete: "queue drains to 0 but nothing loads".
    // 64 MiB gives generous headroom for save + backlog. It's a cap, not a preallocation.
    private const int defaultBufferSize = 67108864; // 64 MiB
    private const int defaultSendRateMin = 4194304; // 4 MiB/s
    private const int defaultSendRateMax = 8388608; // 8 MiB/s

    public static SteamNetworkingConfigValue_t SendBufferSize(int size = defaultBufferSize) => new() {
        m_eValue = ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_SendBufferSize,
        m_eDataType = ESteamNetworkingConfigDataType.k_ESteamNetworkingConfig_Int32,
        m_val = new SteamNetworkingConfigValue_t.OptionValue { m_int32 = size }
    };

    // Raise the LOWER bound of GameNetworkingSockets' send pacing. The estimator starts the connection at
    // SendRateMin and only ramps toward SendRateMax on sustained successful delivery — so a short burst
    // like the hard-sync save (~700 KiB, done in seconds) finishes long before it climbs off the default
    // 128 KiB/s floor. That default floor is exactly why a fresh 1 Gbps/LAN link only pushes ~250 KiB/s.
    // Lifting the floor lets a good link start near full speed; the estimator still backs off on loss, so
    // this never forces traffic onto a bad link.
    public static SteamNetworkingConfigValue_t SendRateMin(int bytesPerSec = defaultSendRateMin) => new() {
        m_eValue = ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_SendRateMin,
        m_eDataType = ESteamNetworkingConfigDataType.k_ESteamNetworkingConfig_Int32,
        m_val = new SteamNetworkingConfigValue_t.OptionValue { m_int32 = bytesPerSec }
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

    // Allow the connection to negotiate a direct (ICE / peer-to-peer) transport instead of only relaying
    // through Valve's SDR datacenters. The relay is both latency-heavy (a ~29 ms round trip shows up even
    // between two machines on the same LAN) and bandwidth-throttled per connection, so a direct path is a
    // large win for the hard-sync transfer. _All lets ICE use private (LAN) and public candidates; it falls
    // back to relay automatically when no direct path can be established, so this is safe to always enable.
    public static SteamNetworkingConfigValue_t IceEnable(
        int flags = k_nSteamNetworkingConfig_P2P_Transport_ICE_Enable_All
    ) => new() {
        m_eValue = ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_P2P_Transport_ICE_Enable,
        m_eDataType = ESteamNetworkingConfigDataType.k_ESteamNetworkingConfig_Int32,
        m_val = new SteamNetworkingConfigValue_t.OptionValue { m_int32 = flags }
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
