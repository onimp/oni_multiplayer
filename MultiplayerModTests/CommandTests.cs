using System;
using System.Runtime.InteropServices;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Network;
using MultiplayerMod.Network.Messaging;
using NUnit.Framework;

namespace MultiplayerModTests;

[TestFixture]
public class CommandTests {

    [Serializable]
    class Command : IMultiplayerCommand {
        public int Value { set; get; }

        public void Execute() { }
    }

    [Test]
    public void TestSerializationDeserialization() {
        var command = new Command { Value = 42 };
        var serialized = NetworkSerializer.Serialize(new NetworkMessage(command, MultiplayerCommandOptions.None));

        byte[] data = new byte[serialized.GetSize()];
        Marshal.Copy(serialized.GetPointer(), data, 0, (int)serialized.GetSize());

        serialized.Dispose();

        var dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
        var messageHandle = new NetworkMessageHandle(dataHandle.AddrOfPinnedObject(), data.Length);

        var message = NetworkSerializer.Deserialize(messageHandle);

        Assert.AreNotSame(command, message.Command);
        Assert.AreEqual(((Command)message.Command).Value, 42);
    }

}
