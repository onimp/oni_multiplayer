using System;
using System.Collections.Generic;

namespace MultiplayerMod.Multiplayer.World.Data;

[Serializable]
public class WorldState {
    public Dictionary<string, object> Entries { get; } = new();
}
