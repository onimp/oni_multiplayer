using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MultiplayerMod.Platform.Steam.Network.Messaging.Surrogates;

namespace MultiplayerMod.Platform.Steam.Network.Messaging;

public static class NetworkSerializer {

    public static SerializedNetworkMessage Serialize(INetworkMessage message) {
        return new SerializedNetworkMessage(message);
    }

    public static unsafe INetworkMessage Deserialize(INetworkMessageHandle message) =>
        (INetworkMessage) CreateFormatter().Deserialize(
            new UnmanagedMemoryStream((byte*) message.Pointer.ToPointer(), message.Size)
        );

    /// <summary>
    /// A <see cref="BinaryFormatter"/> wired with the game-type surrogates and the
    /// <see cref="NetworkMessageSerializationBinder"/> type allowlist. The binder is the RCE mitigation:
    /// it restricts deserialization of untrusted peer data to the types the protocol legitimately sends.
    /// Every deserialize site MUST use this so fragmented messages cannot bypass the allowlist.
    /// </summary>
    public static BinaryFormatter CreateFormatter() => new() {
        SurrogateSelector = SerializationSurrogates.Selector,
        Binder = NetworkMessageSerializationBinder.Instance
    };

}
