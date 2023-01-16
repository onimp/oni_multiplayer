using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace MultiplayerMod.steam
{
    public class SerializedMessage : IDisposable
    {
        public readonly IntPtr IntPtr;
        public uint Size { get; }

        public SerializedMessage(Command command, object payload)
        {
            var bf = new BinaryFormatter();
            using var ms = new MemoryStream();
            bf.Serialize(ms, new TypedMessage(command, payload));
            var array = ms.ToArray();
            Size = (uint)array.Length;
            IntPtr = Marshal.AllocHGlobal(array.Length);
            Marshal.Copy(array, 0, IntPtr, array.Length);
        }

        public SerializedMessage(TypedMessage typedMessage)
        {
            var bf = new BinaryFormatter();
            using var ms = new MemoryStream();
            bf.Serialize(ms, typedMessage);
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
        public struct TypedMessage
        {
            public Command Command { get; set; }
            public object Payload { get; set; }

            public TypedMessage(Command command, object payload)
            {
                Command = command;
                Payload = payload;
            }

            public static TypedMessage DeserializeMessage(IntPtr handle, int size)
            {
                unsafe
                {
                    var memoryStream = new UnmanagedMemoryStream((byte*)handle.ToPointer(), size);
                    var binaryFormatter = new BinaryFormatter();
                    return (TypedMessage)binaryFormatter.Deserialize(memoryStream);
                }
            }
        }
    }
}