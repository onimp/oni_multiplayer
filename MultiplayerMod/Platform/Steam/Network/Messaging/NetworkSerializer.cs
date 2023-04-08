using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MultiplayerMod.Platform.Steam.Network.Messaging.Surrogates;

namespace MultiplayerMod.Platform.Steam.Network.Messaging;

public static class NetworkSerializer {

    public static SerializedNetworkMessage Serialize(INetworkMessage message) {
        return new SerializedNetworkMessage(message);
    }

    public static unsafe INetworkMessage Deserialize(INetworkMessageHandle message) {
        return (INetworkMessage) new BinaryFormatter { SurrogateSelector = SerializationSurrogates.Selector }.Deserialize(
            new UnmanagedMemoryStream((byte*) message.Pointer.ToPointer(), message.Size)
        );
    }

}
