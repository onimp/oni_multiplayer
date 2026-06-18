using System;
using System.Linq;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.World;
using MultiplayerMod.Multiplayer.World.Commands;
using MultiplayerMod.Multiplayer.World.Data;
using NUnit.Framework;

namespace MultiplayerMod.Test.Multiplayer.World;

[TestFixture]
public class WorldSaveTransferManagerTests {

    [Test]
    public void CreatesStartChunkAndCompleteCommandsForLargeSave() {
        var manager = new WorldSaveTransferManager();
        var source = CreateWorldSave(WorldSaveTransferManager.ChunkSize * 2 + 123);

        var commands = manager.CreateCommands(source).ToArray();

        Assert.IsInstanceOf<WorldSaveTransferStart>(commands.First());
        Assert.AreEqual(3, commands.OfType<WorldSaveTransferChunk>().Count());
        Assert.IsInstanceOf<WorldSaveTransferComplete>(commands.Last());
        Assert.IsTrue(commands.All(it => it is IMultiplayerCommand));
    }

    [Test]
    public void ReassemblesLargeSaveInOrder() {
        var source = CreateWorldSave(WorldSaveTransferManager.ChunkSize * 2 + 123);
        var result = Transfer(source);

        Assert.AreEqual(source.Name, result.Name);
        CollectionAssert.AreEqual(source.Data, result.Data);
    }

    [Test]
    public void ReassemblesLargeSaveOutOfOrder() {
        var source = CreateWorldSave(WorldSaveTransferManager.ChunkSize * 2 + 123);
        var result = Transfer(source, reverseChunks: true);

        CollectionAssert.AreEqual(source.Data, result.Data);
    }

    [Test]
    public void DuplicateChunkWithSameDataIsIgnored() {
        var source = CreateWorldSave(WorldSaveTransferManager.ChunkSize + 1);
        var id = Guid.NewGuid();
        var chunks = Split(source.Data).ToArray();
        var manager = StartTransfer(id, source, chunks.Length);

        manager.Append(id, 0, chunks[0]);
        manager.Append(id, 0, chunks[0]);
        manager.Append(id, 1, chunks[1]);

        CollectionAssert.AreEqual(source.Data, manager.Complete(id).Data);
    }

    [Test]
    public void MissingChunkIsRejected() {
        var source = CreateWorldSave(WorldSaveTransferManager.ChunkSize + 1);
        var id = Guid.NewGuid();
        var chunks = Split(source.Data).ToArray();
        var manager = StartTransfer(id, source, chunks.Length);

        manager.Append(id, 0, chunks[0]);

        Assert.Throws<WorldSaveTransferException>(() => manager.Complete(id));
    }

    [Test]
    public void BadHashIsRejected() {
        var id = Guid.NewGuid();
        var manager = new WorldSaveTransferManager();
        manager.Start(id, "bad", new WorldState(), 3, 3, 1, "not-a-real-hash");
        manager.Append(id, 0, new byte[] { 1, 2, 3 });

        Assert.Throws<WorldSaveTransferException>(() => manager.Complete(id));
    }

    [Test]
    public void DuplicateChunkWithDifferentDataIsRejected() {
        var id = Guid.NewGuid();
        var source = CreateWorldSave(3);
        var manager = StartTransfer(id, source, chunkCount: 1);

        manager.Append(id, 0, new byte[] { 1, 2, 3 });

        Assert.Throws<WorldSaveTransferException>(() => manager.Append(id, 0, new byte[] { 1, 2, 4 }));
    }

    private static WorldSave Transfer(WorldSave source, bool reverseChunks = false) {
        var id = Guid.NewGuid();
        var chunks = Split(source.Data).ToArray();
        var manager = StartTransfer(id, source, chunks.Length);

        var indexedChunks = chunks.Select((data, index) => (data, index));
        if (reverseChunks)
            indexedChunks = indexedChunks.Reverse();

        foreach (var (data, index) in indexedChunks)
            manager.Append(id, index, data);

        return manager.Complete(id);
    }

    private static WorldSaveTransferManager StartTransfer(Guid id, WorldSave source, int chunkCount) {
        var manager = new WorldSaveTransferManager();
        manager.Start(
            id,
            source.Name,
            source.State,
            source.Data.LongLength,
            WorldSaveTransferManager.ChunkSize,
            chunkCount,
            WorldSaveTransferManager.ComputeSha256(source.Data)
        );
        return manager;
    }

    private static byte[][] Split(byte[] data) {
        var chunkCount = Math.Max(1, (int) Math.Ceiling(data.Length / (double) WorldSaveTransferManager.ChunkSize));
        return Enumerable.Range(0, chunkCount)
            .Select(index => {
                var offset = index * WorldSaveTransferManager.ChunkSize;
                var length = Math.Min(WorldSaveTransferManager.ChunkSize, data.Length - offset);
                var chunk = new byte[length];
                Buffer.BlockCopy(data, offset, chunk, 0, length);
                return chunk;
            })
            .ToArray();
    }

    private static WorldSave CreateWorldSave(int bytes) {
        var data = Enumerable.Range(0, bytes).Select(it => (byte) (it % 251)).ToArray();
        return new WorldSave("test-world", data, new WorldState());
    }

}
