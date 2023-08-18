using System;
using System.Linq;
using System.Runtime.InteropServices;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Network;
using MultiplayerMod.Platform.Steam.Network;
using MultiplayerMod.Platform.Steam.Network.Messaging;
using NUnit.Framework;

namespace MultiplayerModTests.Network;

[TestFixture]
public class CommandTests {

    [Serializable]
    private class Command : MultiplayerCommand {
        public int Value { set; get; }

        public override void Execute() { }
    }

    [Serializable]
    private class DataCommand : MultiplayerCommand {
        public byte[] Data = new byte[Configuration.MaxMessageSize * 2];

        public override void Execute() { }
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

}
