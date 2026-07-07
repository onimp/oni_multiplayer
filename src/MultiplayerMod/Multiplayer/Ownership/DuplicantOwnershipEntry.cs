using System;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Players;

namespace MultiplayerMod.Multiplayer.Ownership;

/// <summary>
/// Wire-friendly (proxy id -> owner) pair. Used instead of <c>KeyValuePair</c> in the snapshot sync command
/// so serialization stays on the same explicit-type footing as the rest of the command payloads.
/// </summary>
[Serializable]
public record DuplicantOwnershipEntry(MultiplayerId Id, PlayerIdentity Owner);
