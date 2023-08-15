using System;
using MultiplayerMod.Multiplayer;

namespace MultiplayerMod.Platform.Gns.Network;

[Serializable]
public record DevPlayer(string Id) : IPlayer {
    public bool Equals(IPlayer other) {
        return other is DevPlayer player && player.Equals(this);
    }
}
