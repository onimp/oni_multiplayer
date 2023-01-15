using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace MultiplayerMod.steam
{
    public class ServerToClientEnvelope : IDisposable
    {
        public readonly IntPtr IntPtr;
        public uint Size { get; }

        public ServerToClientEnvelope(ServerToClientMessage serverToClientMessage)
        {
            var bf = new BinaryFormatter();
            using var ms = new MemoryStream();
            bf.Serialize(ms, serverToClientMessage);
            var array = ms.ToArray();
            Size = (uint)array.Length;
            IntPtr = Marshal.AllocHGlobal(array.Length);
            Marshal.Copy(array, 0, IntPtr, array.Length);
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(IntPtr);
        }

        [Serializable]
        public struct ServerToClientMessage
        {
            public Command Command { get; set; }
            public object Payload { get; set; }

            public ServerToClientMessage(Command command, object payload)
            {
                Command = command;
                Payload = payload;
            }

            public static ServerToClientMessage ToServerToClientMessage(IntPtr handle, int size)
            {
                unsafe
                {
                    var memoryStream = new UnmanagedMemoryStream((byte*)handle.ToPointer(), size);
                    var binaryFormatter = new BinaryFormatter();
                    return (ServerToClientMessage)binaryFormatter.Deserialize(memoryStream);
                }
            }
        }
    }
}