using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MultiplayerMod.Network.Messaging;

public static class NetworkSerializer {

    public static INetworkMessageHandle Serialize(NetworkMessage message) {
        return new SerializedNetworkMessage(message);
    }

    public static unsafe NetworkMessage Deserialize(INetworkMessageHandle message) {
        return (NetworkMessage) new BinaryFormatter().Deserialize(
            new UnmanagedMemoryStream((byte*) message.GetPointer().ToPointer(), message.GetSize())
        );
    }

}
