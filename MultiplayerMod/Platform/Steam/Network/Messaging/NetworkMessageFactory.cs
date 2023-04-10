using System;
using System.Collections.Generic;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Network;
using static MultiplayerMod.Platform.Steam.Network.Configuration;

namespace MultiplayerMod.Platform.Steam.Network.Messaging;

public class NetworkMessageFactory {

    public IEnumerable<INetworkMessageHandle> Create(IMultiplayerCommand command, MultiplayerCommandOptions options) {
        using var message = NetworkSerializer.Serialize(new NetworkMessage(command, options));
        if (message.Size <= MaxMessageSize) {
            yield return message;
            yield break;
        }

        var fragmentsCount = (int) message.Size / MaxFragmentDataSize + 1;
        var header = new NetworkMessageFragmentsHeader(fragmentsCount);
        var serializedHeader = NetworkSerializer.Serialize(header);
        yield return serializedHeader;

        for (var i = 0; i < fragmentsCount; i++) {
            var offset = i * MaxFragmentDataSize;
            var bufferSize = Math.Min(Math.Max((int) message.Size - offset, 0), MaxFragmentDataSize);
            var data = new byte[bufferSize];
            Buffer.BlockCopy(message.GetBuffer(), offset, data, 0, bufferSize);
            using var serialized = NetworkSerializer.Serialize(new NetworkMessageFragment(header.MessageId, data));
            yield return serialized;
        }
    }

}
