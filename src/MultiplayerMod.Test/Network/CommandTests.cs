using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
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

        var data = new byte[serialized.Size];
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

        var fragmentsCount = 0;
        var message = factory.Create(command, MultiplayerCommandOptions.None)
            .Select(
                fragment => {
                    fragmentsCount++;
                    return processor.Process(0, fragment);
                }
            )
            .FirstOrDefault(it => it != null);

        Assert.AreEqual(4, fragmentsCount);
        Assert.NotNull(message);
    }

    [Test]
    public void BinderAllowsFirstPartyAndProtocolTypes() {
        Assert.IsTrue(NetworkMessageSerializationBinder.IsAllowedType(typeof(Command)));
        Assert.IsTrue(NetworkMessageSerializationBinder.IsAllowedType(typeof(NetworkMessage)));
        Assert.IsTrue(NetworkMessageSerializationBinder.IsAllowedType(typeof(int[])));
    }

    [Test]
    public void BinderRejectsDisallowedType() {
        var dangerous = typeof(System.Diagnostics.Process);
        Assert.IsFalse(NetworkMessageSerializationBinder.IsAllowedType(dangerous));
        Assert.Throws<SerializationException>(
            () => NetworkMessageSerializationBinder.Instance.BindToType(dangerous.Assembly.FullName, dangerous.FullName)
        );
    }

    [Test]
    public void ProcessDropsMalformedMessageInsteadOfThrowing() {
        // A malformed / disallowed payload must not tear down the receive loop — Process returns null.
        var processor = new NetworkMessageProcessor();
        var garbage = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var handle = GCHandle.Alloc(garbage, GCHandleType.Pinned);
        try {
            var message = processor.Process(0, new NetworkMessageHandle(handle.AddrOfPinnedObject(), (uint) garbage.Length));
            Assert.IsNull(message);
        } finally {
            handle.Free();
        }
    }

}
