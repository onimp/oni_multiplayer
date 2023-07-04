using System;

namespace MultiplayerMod.Multiplayer.Objects;

[Serializable]
public record MultiplayerId(IPlayer Player, long ObjectId);
