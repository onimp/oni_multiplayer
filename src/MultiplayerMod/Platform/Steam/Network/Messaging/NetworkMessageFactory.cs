using System;
using System.Collections.Generic;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Network;
using static MultiplayerMod.Platform.Steam.Network.Configuration;

namespace MultiplayerMod.Platform.Steam.Network.Messaging;

public class NetworkMessageFactory {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<NetworkMessageFactory>();

    public IEnumerable<INetworkMessageHandle> Create(IMultiplayerCommand command, MultiplayerCommandOptions options) {
        using var message = NetworkSerializer.Serialize(new NetworkMessage(command, options));
        if (message.Size <= MaxMessageSize) {
            yield return message;
            yield break;
        }

        var fragmentsCount = (int) message.Size / MaxFragmentDataSize + 1;
        var header = new NetworkMessageFragmentsHeader(fragmentsCount);
        log.Info(
            $"Fragmenting {command.GetType().Name} ({message.Size / 1024}KiB) into {fragmentsCount} fragments " +
            $"(message id {header.MessageId})"
        );
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
