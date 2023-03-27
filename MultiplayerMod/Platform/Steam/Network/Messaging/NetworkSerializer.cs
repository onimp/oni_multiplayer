using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MultiplayerMod.Platform.Steam.Network.Messaging;

public static class NetworkSerializer {

    public static SerializedNetworkMessage Serialize(INetworkMessage message) {
        return new SerializedNetworkMessage(message);
    }

    public static unsafe INetworkMessage Deserialize(INetworkMessageHandle message) {
        return (INetworkMessage) new BinaryFormatter().Deserialize(
            new UnmanagedMemoryStream((byte*) message.Pointer.ToPointer(), message.Size)
        );
    }

}
