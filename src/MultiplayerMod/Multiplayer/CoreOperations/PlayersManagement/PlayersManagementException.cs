using MultiplayerMod.Exceptions;

namespace MultiplayerMod.Multiplayer.CoreOperations.PlayersManagement;

public class PlayersManagementException : MultiplayerException {
    public PlayersManagementException(string message) : base(message) { }
}
