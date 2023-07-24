using System;
using System.Collections.Generic;
using MultiplayerMod.Core.Extensions;

namespace MultiplayerMod.Multiplayer.State;

[Serializable]
public class MultiplayerState {
    public Dictionary<IPlayer, PlayerState> Players = new();

    public void Rebuild() {
        Players.Values.ForEach(playerState => playerState.Spawned = false);
    }
}
