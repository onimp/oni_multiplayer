using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MultiplayerMod.Network.Command {

    public static class CommandSerializer {

        public static ISerializedCommand Serialize(ICommand command) {
            return new SerializedCommand(command);
        }

        public static unsafe ICommand Deserialize(ISerializedCommand binary) {
            return (ICommand) new BinaryFormatter().Deserialize(
                new UnmanagedMemoryStream((byte*)binary.GetPointer().ToPointer(), binary.GetSize())
            );
        }

    }

}
