using MultiplayerMod.Multiplayer;

namespace MultiplayerMod.Network.Events;

public class PlayerConnectedEventArgs {
    public IPlayer Player { get; }

    public PlayerConnectedEventArgs(IPlayer player) {
        Player = player;
    }
}
