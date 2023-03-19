using MultiplayerMod.Multiplayer;
using MultiplayerMod.Network;
using Steamworks;

namespace MultiplayerMod.Platform.Steam.Network;

public record SteamPlayer(CSteamID Id) : IPlayer {

    public bool Equals(IPlayer other) {
        return other is SteamPlayer player && player.Equals(this);
    }

}
