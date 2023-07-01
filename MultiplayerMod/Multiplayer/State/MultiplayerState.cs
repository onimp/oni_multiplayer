using System;
using System.Collections.Generic;

namespace MultiplayerMod.Multiplayer.State;

[Serializable]
public class MultiplayerState {
    public Dictionary<IPlayer, PlayerState> Players = new();
}
