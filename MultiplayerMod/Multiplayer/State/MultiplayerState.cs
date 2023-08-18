using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.State;

[Serializable]
[UsedImplicitly]
public class MultiplayerState  {

    public Dictionary<IPlayerIdentity, PlayerState> Players = new();

    [Dependency]
    [NonSerialized]
    private IMultiplayerClient client;

    public MultiplayerState(IMultiplayerClient client) {
        this.client = client;
    }

    public PlayerState Current => Players[client.Player];

}
