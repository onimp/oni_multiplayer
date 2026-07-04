namespace MultiplayerMod.Network;

/// <summary>
/// Steam priority lane a command's network traffic rides on. Lanes are delivered by priority
/// (lower value first) and are independent of each other, so bulk traffic on a low-priority lane
/// never head-of-line-blocks latency-sensitive gameplay on the high-priority lane.
/// The enum value is the Steam lane index (see <c>ConfigureConnectionLanes</c>).
/// </summary>
public enum NetworkLane {

    /// <summary>
    /// Highest priority, reliable. Latency-sensitive gameplay: movement, pausing, chore
    /// creation/assignment, tool orders, building config. The default for every command.
    /// </summary>
    Gameplay = 0,

    /// <summary>
    /// Low priority, sent unreliable. Idempotent core-sim cell corrections that are re-sent every
    /// sweep, so a dropped packet self-heals and reliability/retransmit is pure overhead. Commands on
    /// this lane MUST serialize to a single (unfragmented) network message.
    /// </summary>
    Sim = 1,

    /// <summary>
    /// Lowest priority, reliable. Large, infrequent payloads that must arrive complete: the hard-sync
    /// save transfer and the world debug snapshot.
    /// </summary>
    Bulk = 2

}
