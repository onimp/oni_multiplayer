using MultiplayerMod.Exceptions;

namespace MultiplayerMod.Multiplayer.Configuration;

public class MultiplayerConnectionException : MultiplayerException {
    public MultiplayerConnectionException(string message) : base(message) { }
}
