using System;
using System.Linq;
using System.Runtime.InteropServices;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Network;
using MultiplayerMod.Platform.Steam.Network;
using MultiplayerMod.Platform.Steam.Network.Messaging;
using NUnit.Framework;

namespace MultiplayerMod.Test.Network;

[TestFixture]
[Parallelizable]
public class CommandTests {

    [Serializable]
    private class Command : MultiplayerCommand {
        public int Value { set; get; }

        public override void Execute(MultiplayerCommandContext context) { }
    }

    [Serializable]
    private class DataCommand : MultiplayerCommand {
        public byte[] Data = new byte[Configuration.MaxMessageSize * 2];

        public override void Execute(MultiplayerCommandContext context) { }
    }

    [Test]
    public void TestSerializationDeserialization() {
        var command = new Command { Value = 42 };
        using var serialized = NetworkSerializer.Serialize(new NetworkMessage(command, MultiplayerCommandOptions.None));

        var data = new byte[checked((int) serialized.Size)];
        Marshal.Copy(serialized.Pointer, data, 0, (int) serialized.Size);

        var dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
        var messageHandle = new NetworkMessageHandle(dataHandle.AddrOfPinnedObject(), (uint) data.Length);

        var message = (NetworkMessage) NetworkSerializer.Deserialize(messageHandle);

        Assert.AreNotSame(command, message.Command);
        Assert.AreEqual(((Command) message.Command).Value, 42);
    }

    [Test]
    public void TestNetworkMessageFragmentation() {
        var factory = new NetworkMessageFactory();
        var processor = new NetworkMessageProcessor();
        var command = new DataCommand();

        var fragments = factory.Create(command, MultiplayerCommandOptions.None)
            .Select(Copy)
            .ToArray();
        var message = fragments
            .Select(fragment => Process(processor, fragment))
            .FirstOrDefault(it => it != null);

        Assert.Greater(fragments.Length, 1);
        Assert.NotNull(message);
    }

    [Test]
    public void TestNetworkMessageFragmentationOutOfOrder() {
        var factory = new NetworkMessageFactory();
        var processor = new NetworkMessageProcessor();
        var command = new DataCommand();

        var fragments = factory.Create(command, MultiplayerCommandOptions.None)
            .Select(Copy)
            .ToArray();
        var header = fragments.First();
        var dataFragments = fragments.Skip(1).Reverse();

        Process(processor, header);
        var message = dataFragments
            .Select(fragment => Process(processor, fragment))
            .FirstOrDefault(it => it != null);

        Assert.Greater(fragments.Length, 1);
        Assert.NotNull(message);
    }

    private static byte[] Copy(INetworkMessageHandle handle) {
        var data = new byte[checked((int) handle.Size)];
        Marshal.Copy(handle.Pointer, data, 0, (int) handle.Size);
        return data;
    }

    private static NetworkMessage? Process(NetworkMessageProcessor processor, byte[] data) {
        var dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
        try {
            return processor.Process(0, new NetworkMessageHandle(dataHandle.AddrOfPinnedObject(), (uint) data.Length));
        } finally {
            dataHandle.Free();
        }
    }

}
