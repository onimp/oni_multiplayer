using MultiplayerMod.Exceptions;

namespace MultiplayerMod.Multiplayer.World;

public class WorldSaveTransferException : MultiplayerException {
    public WorldSaveTransferException(string message) : base(message) { }
}
