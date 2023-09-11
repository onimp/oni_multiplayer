using MultiplayerMod.Exceptions;

namespace MultiplayerMod.Multiplayer.CoreOperations.CommandExecution;

public class CommandConfigurationException : MultiplayerException {
    public CommandConfigurationException(string message) : base(message) { }
}
