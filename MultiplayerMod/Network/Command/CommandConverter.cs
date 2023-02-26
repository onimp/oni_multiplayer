using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MultiplayerMod.Network.Command {

    public static class CommandConverter {

        public static IBinaryCommand Convert(ICommand command) {
            return new BinaryCommand(command);
        }

        public static unsafe ICommand Convert(IBinaryCommand binary) {
            return (ICommand) new BinaryFormatter().Deserialize(
                new UnmanagedMemoryStream((byte*)binary.GetPointer().ToPointer(), binary.GetSize())
            );
        }

    }

}
