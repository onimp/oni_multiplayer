using System;

namespace MultiplayerMod.Platform.Base.Network.Messaging;

[Serializable]
public class NetworkMessageFragment : INetworkMessage {

    public int MessageId { get; }
    public byte[] Data { get; }

    public NetworkMessageFragment(int messageId, byte[] data) {
        MessageId = messageId;
        Data = data;
    }

}
