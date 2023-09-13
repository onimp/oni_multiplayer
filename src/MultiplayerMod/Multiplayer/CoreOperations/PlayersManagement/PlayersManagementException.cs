using MultiplayerMod.Exceptions;

namespace MultiplayerMod.Multiplayer.CoreOperations;

public class PlayersManagementException : MultiplayerException {
    public PlayersManagementException(string message) : base(message) { }
}
