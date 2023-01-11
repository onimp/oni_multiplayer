using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace MultiplayerMod.steam
{
    public class ServerToClientEnvelope : IDisposable
    {
        private GCHandle _handle;

        public IntPtr IntPtr => (IntPtr)_handle;
        public uint Size { get; }

        public ServerToClientEnvelope(ServerToClientMessage serverToClientMessage)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using var ms = new MemoryStream();
            bf.Serialize(ms, serverToClientMessage);
            var array = ms.ToArray();
            _handle = GCHandle.Alloc(array, GCHandleType.Pinned);
            Size = (uint)array.Length;
        }

        public void Dispose()
        {
            _handle.Free();
        }

        [Serializable]
        public class ServerToClientMessage
        {
            public Command Command { get; set; }
            public object Payload { get; set; }

            public ServerToClientMessage(Command command, object payload)
            {
                Command = command;
                Payload = payload;
            }

            public static ServerToClientMessage ToServerToClientMessage(IntPtr handle)
            {
                return (ServerToClientMessage)Marshal.PtrToStructure(handle, typeof(ServerToClientMessage));
            }
        }
    }
}