using System;
using System.Collections.Concurrent;
using System.Linq;
using MultiplayerMod.Core.Logging;
using static MultiplayerMod.Platform.Steam.Network.Configuration;

namespace MultiplayerMod.Platform.Steam.Network.Messaging;

public class NetworkMessageProcessor {

    private readonly ConcurrentDictionary<uint, ConcurrentDictionary<int, FragmentsBuffer>> fragments = new();
    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<NetworkMessageProcessor>();

    public NetworkMessage? Process(uint clientId, INetworkMessageHandle handle) =>
        NetworkSerializer.Deserialize(handle) switch {
            NetworkMessage message => message,
            NetworkMessageFragmentsHeader header => ProcessFragmentsHeader(clientId, header),
            NetworkMessageFragment fragment => ProcessMessageFragment(clientId, fragment),
            _ => null
        };

    private NetworkMessage? ProcessFragmentsHeader(uint clientId, NetworkMessageFragmentsHeader header) {
        fragments.TryGetValue(clientId, out var index);
        if (index == null) {
            index = new ConcurrentDictionary<int, FragmentsBuffer>();
            fragments[clientId] = index;
        }
        var buffer = new FragmentsBuffer(header.FragmentsCount, header.TotalSize);
        buffer.Timeout += () => {
            log.Warning($"Fragments buffer timed out (message id: {header.MessageId})");
            index.TryRemove(header.MessageId, out _);
        };
        index[header.MessageId] = buffer;
        return null;
    }

    private NetworkMessage? ProcessMessageFragment(uint clientId, NetworkMessageFragment fragment) {
        string ExceptionMessage() =>
            $"Message (id: {fragment.MessageId}) fragment received, but no fragments buffer found";

        if (!fragments.TryGetValue(clientId, out var index)) {
            log.Warning(ExceptionMessage());
            return null;
        }

        if (!index.TryGetValue(fragment.MessageId, out var buffer)) {
            log.Warning(ExceptionMessage());
            return null;
        }

        var message = buffer.Append(fragment);
        if (message != null)
            index.TryRemove(fragment.MessageId, out _);

        return message;
    }

    private class FragmentsBuffer {
        private const int watchdogIntervalMs = 5000;

        private readonly int count;
        private readonly int totalSize;
        private readonly byte[]?[] chunks;
        private readonly byte[] buffer;
        private int receivedCount;
        private int receivedSize;

        public event System.Action? Timeout;

        private readonly System.Timers.Timer watchdog = new(watchdogIntervalMs) {
            Enabled = true,
            AutoReset = false
        };

        public FragmentsBuffer(int count, int totalSize) {
            if (count <= 0 || totalSize < 0)
                throw new NetworkPlatformException("Invalid fragmentation header.");

            this.count = count;
            this.totalSize = totalSize;
            chunks = new byte[]?[count];
            buffer = new byte[totalSize];
            watchdog.Elapsed += (_, _) => Timeout?.Invoke();
        }

        public NetworkMessage? Append(NetworkMessageFragment fragment) {
            if (fragment.FragmentIndex < 0 || fragment.FragmentIndex >= count)
                throw new NetworkPlatformException("Invalid fragmentation: fragment index is outside the expected range.");

            var existing = chunks[fragment.FragmentIndex];
            if (existing != null) {
                if (!existing.SequenceEqual(fragment.Data))
                    throw new NetworkPlatformException("Invalid fragmentation: duplicate fragment has different data.");
                return null;
            }

            watchdog.Interval = watchdogIntervalMs;
            chunks[fragment.FragmentIndex] = fragment.Data;
            receivedCount++;
            receivedSize += fragment.Data.Length;
            if (receivedCount != count)
                return null;

            watchdog.Enabled = false;
            if (receivedSize != totalSize)
                throw new NetworkPlatformException($"Invalid fragmentation: expected {totalSize} bytes, received {receivedSize} bytes.");

            var offset = 0;
            for (var i = 0; i < chunks.Length; i++) {
                var chunk = chunks[i];
                if (chunk == null)
                    throw new NetworkPlatformException($"Invalid fragmentation: fragment {i} is missing.");
                Buffer.BlockCopy(chunk, 0, buffer, offset, chunk.Length);
                offset += chunk.Length;
            }

            return (NetworkMessage) NetworkSerializer.Deserialize(buffer, totalSize);
        }
    }

}
