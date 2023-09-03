using System;
using MultiplayerMod.Network;
using Steamworks;

namespace MultiplayerMod.Platform.Steam.Network;

[Serializable]
public record SteamMultiplayerClientId(CSteamID Id) : IMultiplayerClientId {

    public bool Equals(IMultiplayerClientId other) {
        return other is SteamMultiplayerClientId player && player.Equals(this);
    }

}
