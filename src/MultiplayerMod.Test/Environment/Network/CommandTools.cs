using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Platform.Steam.Network.Messaging.Surrogates;

namespace MultiplayerMod.Test.Environment.Network;

public static class CommandTools {

    public static IMultiplayerCommand Copy(IMultiplayerCommand command) {
        using var memory = new MemoryStream();
        var formatter = new BinaryFormatter { SurrogateSelector = SerializationSurrogates.Selector };
        formatter.Serialize(memory, command);
        memory.Position = 0;
        return (IMultiplayerCommand) formatter.Deserialize(memory);
    }

}
