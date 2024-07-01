using System;

namespace MultiplayerMod.Multiplayer.World.Data;

[Serializable]
public record WorldSave(string Name, byte[] Data, WorldState State);
