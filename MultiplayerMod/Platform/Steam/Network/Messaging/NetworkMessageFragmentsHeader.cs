using System;
using System.Threading;

namespace MultiplayerMod.Platform.Steam.Network.Messaging;

[Serializable]
public class NetworkMessageFragmentsHeader : INetworkMessage {

    public int MessageId { get; }
    public int FragmentsCount { get; }

    private static int uniqueMessageId;

    public NetworkMessageFragmentsHeader(int fragmentsCount) {
        MessageId = Interlocked.Increment(ref uniqueMessageId);
        FragmentsCount = fragmentsCount;
    }

}
