using System;
using MultiplayerMod.Multiplayer;
using Steamworks;

namespace MultiplayerMod.Platform.Steam.Network;

[Serializable]
public record SteamPlayer(CSteamID Id) : IPlayer {

    public bool Equals(IPlayer other) {
        return other is SteamPlayer player && player.Equals(this);
    }

}

[Serializable]
public record DevPlayer(string Id) : IPlayer {
    public bool Equals (IPlayer other)
    {
        return other is DevPlayer player && player.Equals(this);
    }
}