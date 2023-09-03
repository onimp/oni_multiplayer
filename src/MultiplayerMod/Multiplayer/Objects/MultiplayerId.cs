using System;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Objects;

[Serializable]
public record MultiplayerId(IMultiplayerClientId? Player, long ObjectId);
