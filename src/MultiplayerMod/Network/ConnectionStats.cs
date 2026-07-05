namespace MultiplayerMod.Network;

/// <summary>
/// A snapshot of live Steam connection quality for one endpoint, sampled from
/// <c>GetConnectionRealTimeStatus</c>. On a client this describes the link to the host; on the host it is
/// the aggregate across all client connections. Used by the hard-sync overlay to show real transfer
/// numbers (rates, ping, backlog) instead of only an elapsed timer.
/// </summary>
public readonly struct ConnectionStats {

    /// <summary>Estimated round-trip latency, milliseconds.</summary>
    public int PingMs { get; }

    /// <summary>Current outbound (upload) throughput, bytes per second.</summary>
    public float OutBytesPerSec { get; }

    /// <summary>Current inbound (download) throughput, bytes per second.</summary>
    public float InBytesPerSec { get; }

    /// <summary>Reliable payload still queued/in-flight (not yet acknowledged), bytes. Falls to 0 when the
    /// save transfer has fully landed.</summary>
    public int PendingReliableBytes { get; }

    /// <summary>Local connection quality, 0..1 (1 = no loss).</summary>
    public float ConnectionQuality { get; }

    public ConnectionStats(
        int pingMs,
        float outBytesPerSec,
        float inBytesPerSec,
        int pendingReliableBytes,
        float connectionQuality
    ) {
        PingMs = pingMs;
        OutBytesPerSec = outBytesPerSec;
        InBytesPerSec = inBytesPerSec;
        PendingReliableBytes = pendingReliableBytes;
        ConnectionQuality = connectionQuality;
    }

}
