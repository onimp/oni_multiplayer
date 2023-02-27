using System;
using System.Runtime.InteropServices;
using MultiplayerMod.Network.Command;
using NUnit.Framework;

namespace MultiplayerModTests {

    [TestFixture]
    public class CommandTests {

        [Serializable]
        class Command : ICommand {
            public int Value { set; get; }

            public void Execute() { }
        }

        [Test]
        public void TestSerializationDeserialization() {
            var command = new Command { Value = 10 };
            var serialized = CommandSerializer.Serialize(command);

            byte[] data = new byte[serialized.GetSize()];
            Marshal.Copy(serialized.GetPointer(), data, 0, (int)serialized.GetSize());

            serialized.Dispose();

            var dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            var commandHandle = new SerializedCommandHandle(dataHandle.AddrOfPinnedObject(), data.Length);

            var result = CommandSerializer.Deserialize<Command>(commandHandle);

            Assert.AreNotSame(command, result);
            Assert.AreEqual(result.Value, 10);
        }

    }

}
