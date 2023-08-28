using System;
using MultiplayerMod.Multiplayer;
using Steamworks;

namespace MultiplayerMod.Platform.Steam.Network;

[Serializable]
public record SteamPlayerIdentity(CSteamID Id) : IPlayerIdentity {

    public bool Equals(IPlayerIdentity other) {
        return other is SteamPlayerIdentity player && player.Equals(this);
    }

}
