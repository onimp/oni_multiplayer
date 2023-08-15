using System;
using System.Collections.Generic;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.State;

[Serializable]
public class MultiplayerState {
    public Dictionary<IPlayerIdentity, PlayerState> Players = new();

    public PlayerState Current => Players[Dependencies.Get<IMultiplayerClient>().Player];
}
