using System;

namespace MultiplayerMod.Multiplayer.Objects;

[Serializable]
public record MultiplayerId(IPlayerIdentity? Player, long ObjectId);
