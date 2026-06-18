using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.World.Commands;
using MultiplayerMod.Multiplayer.World.Data;

namespace MultiplayerMod.Multiplayer.World;

[Dependency, UsedImplicitly]
public class WorldSaveTransferManager {

    public const int ChunkSize = 256 * 1024;

    private readonly Dictionary<Guid, IncomingTransfer> incoming = new();

    public IEnumerable<IMultiplayerCommand> CreateCommands(WorldSave world) {
        var transferId = Guid.NewGuid();
        var chunkCount = Math.Max(1, (int) Math.Ceiling(world.Data.Length / (double) ChunkSize));
        var hash = ComputeSha256(world.Data);

        yield return new WorldSaveTransferStart(
            transferId,
            world.Name,
            world.State,
            world.Data.LongLength,
            ChunkSize,
            chunkCount,
            hash
        );

        for (var chunkIndex = 0; chunkIndex < chunkCount; chunkIndex++) {
            var offset = chunkIndex * ChunkSize;
            var length = Math.Min(ChunkSize, world.Data.Length - offset);
            var data = new byte[length];
            Buffer.BlockCopy(world.Data, offset, data, 0, length);
            yield return new WorldSaveTransferChunk(transferId, chunkIndex, data);
        }

        yield return new WorldSaveTransferComplete(transferId);
    }

    public void Start(
        Guid transferId,
        string name,
        WorldState state,
        long totalBytes,
        int chunkSize,
        int chunkCount,
        string sha256
    ) {
        if (totalBytes < 0)
            throw new WorldSaveTransferException("World save transfer size cannot be negative.");
        if (chunkSize <= 0)
            throw new WorldSaveTransferException("World save transfer chunk size must be positive.");
        if (chunkCount <= 0)
            throw new WorldSaveTransferException("World save transfer chunk count must be positive.");

        incoming[transferId] = new IncomingTransfer(name, state, totalBytes, chunkSize, chunkCount, sha256);
    }

    public void Append(Guid transferId, int chunkIndex, byte[] data) {
        if (!incoming.TryGetValue(transferId, out var transfer))
            throw new WorldSaveTransferException($"World save transfer {transferId} was not started.");

        transfer.Append(chunkIndex, data);
    }

    public WorldSave Complete(Guid transferId) {
        if (!incoming.TryGetValue(transferId, out var transfer))
            throw new WorldSaveTransferException($"World save transfer {transferId} was not started.");

        try {
            return transfer.Complete();
        } finally {
            incoming.Remove(transferId);
        }
    }

    public static string ComputeSha256(byte[] data) {
        using var sha256 = SHA256.Create();
        return string.Concat(sha256.ComputeHash(data).Select(it => it.ToString("x2")));
    }

    private class IncomingTransfer {

        private readonly string name;
        private readonly WorldState state;
        private readonly long totalBytes;
        private readonly int chunkSize;
        private readonly int chunkCount;
        private readonly string sha256;
        private readonly byte[]?[] chunks;
        private int receivedChunks;

        public IncomingTransfer(
            string name,
            WorldState state,
            long totalBytes,
            int chunkSize,
            int chunkCount,
            string sha256
        ) {
            this.name = name;
            this.state = state;
            this.totalBytes = totalBytes;
            this.chunkSize = chunkSize;
            this.chunkCount = chunkCount;
            this.sha256 = sha256;
            chunks = new byte[chunkCount][];
        }

        public void Append(int chunkIndex, byte[] data) {
            if (chunkIndex < 0 || chunkIndex >= chunkCount)
                throw new WorldSaveTransferException($"Invalid world save transfer chunk index {chunkIndex}.");
            if (data.Length > chunkSize)
                throw new WorldSaveTransferException($"World save transfer chunk {chunkIndex} is too large.");

            var existing = chunks[chunkIndex];
            if (existing != null) {
                if (!existing.SequenceEqual(data))
                    throw new WorldSaveTransferException($"World save transfer chunk {chunkIndex} was received twice with different data.");
                return;
            }

            chunks[chunkIndex] = data;
            receivedChunks++;
        }

        public WorldSave Complete() {
            if (receivedChunks != chunkCount)
                throw new WorldSaveTransferException($"World save transfer is incomplete ({receivedChunks}/{chunkCount} chunks).");

            if (totalBytes > int.MaxValue)
                throw new WorldSaveTransferException($"World save transfer is too large for this runtime: {totalBytes} bytes.");

            var data = new byte[(int) totalBytes];
            var offset = 0;
            for (var i = 0; i < chunks.Length; i++) {
                var chunk = chunks[i];
                if (chunk == null)
                    throw new WorldSaveTransferException($"World save transfer chunk {i} is missing.");
                if (offset + chunk.Length > data.Length)
                    throw new WorldSaveTransferException("World save transfer contains more data than expected.");

                Buffer.BlockCopy(chunk, 0, data, offset, chunk.Length);
                offset += chunk.Length;
            }

            if (offset != totalBytes)
                throw new WorldSaveTransferException($"World save transfer size mismatch: expected {totalBytes}, received {offset}.");
            if (!string.Equals(ComputeSha256(data), sha256, StringComparison.OrdinalIgnoreCase))
                throw new WorldSaveTransferException("World save transfer checksum mismatch.");

            return new WorldSave(name, data, state);
        }

    }

}
