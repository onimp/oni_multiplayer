using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using MultiplayerMod.Platform.Steam.Network.Messaging.Surrogates;

namespace MultiplayerMod.Platform.Steam.Network.Messaging;

public static class NetworkSerializer {

    public static SerializedNetworkMessage Serialize(INetworkMessage message) {
        return new SerializedNetworkMessage(message);
    }

    public static unsafe INetworkMessage Deserialize(INetworkMessageHandle message) {
        if (message.Size == 0)
            throw new NetworkPlatformException("Unable to deserialize empty network message.");

        try {
            return (INetworkMessage) CreateFormatter()
                .Deserialize(new UnmanagedMemoryStream((byte*) message.Pointer.ToPointer(), message.Size));
        } catch (Exception e) when (IsDeserializationFailure(e)) {
            throw new NetworkPlatformException("Unable to deserialize network message.", e);
        }
    }

    public static INetworkMessage Deserialize(byte[] data, int count) {
        if (count == 0)
            throw new NetworkPlatformException("Unable to deserialize empty network message.");

        try {
            using var stream = new MemoryStream(data, 0, count);
            return (INetworkMessage) CreateFormatter().Deserialize(stream);
        } catch (Exception e) when (IsDeserializationFailure(e)) {
            throw new NetworkPlatformException("Unable to deserialize network message.", e);
        }
    }

    private static bool IsDeserializationFailure(Exception e) =>
        e is SerializationException or InvalidCastException or IOException or ArgumentException;

    private static BinaryFormatter CreateFormatter() => new() {
        SurrogateSelector = SerializationSurrogates.Selector
    };

}
