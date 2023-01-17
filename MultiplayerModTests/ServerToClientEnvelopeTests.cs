using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using MultiplayerMod.steam;
using NUnit.Framework;

namespace MultiplayerModTests
{
    [TestFixture]
    public class ServerToClientEnvelopeTests
    {
        [Test]
        public void TestSerialization()
        {
            var message = new SerializedMessage.TypedMessage(Command.PlayersState, new byte[] {123, 45, 12} );
            var envelope = new SerializedMessage(message);
            var managedArray = new byte[envelope.Size];
            Marshal.Copy(envelope.IntPtr, managedArray, 0, (int)envelope.Size);


            var expected = ObjectToByteArray(message);
            Assert.AreEqual(expected, managedArray);
        }
        
        [Test]
        public void TestBackAndForth()
        {
            var message = new SerializedMessage.TypedMessage(Command.PlayersState, "test");
            var envelope = new SerializedMessage(message);

            var message2 = SerializedMessage.TypedMessage.DeserializeMessage(envelope.IntPtr, (int)envelope.Size);

            Assert.AreEqual(message, message2);
        }

        private static byte[] ObjectToByteArray(object obj)
        {
            var bf = new BinaryFormatter();
            using var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }
}