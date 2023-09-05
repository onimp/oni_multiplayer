using MultiplayerMod.Exceptions;

namespace MultiplayerMod.Multiplayer.Configuration;

public class CommandConfigurationException : MultiplayerException {
    public CommandConfigurationException(string message) : base(message) { }
}
