using System;
using System.Collections.Generic;
using System.Linq;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Ownership;
using MultiplayerMod.Multiplayer.Players;

namespace MultiplayerMod.Multiplayer.Commands.Player;

/// <summary>
/// Full ownership snapshot pushed from host to clients, so a late joiner or a client that just finished a
/// hard-sync reconciles its registry mirror in one shot. Replaces the client's whole map and the feature flag.
/// </summary>
[Serializable]
public class SyncDuplicantOwnership : MultiplayerCommand {

    private readonly List<DuplicantOwnershipEntry> entries;
    private readonly bool enabled;

    public SyncDuplicantOwnership(List<DuplicantOwnershipEntry> entries, bool enabled) {
        this.entries = entries;
        this.enabled = enabled;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var registry = context.Dependencies.Get<DuplicantOwnershipRegistry>();
        registry.Replace(entries.Select(it => new KeyValuePair<MultiplayerId, PlayerIdentity>(it.Id, it.Owner)));
        registry.SetEnabled(enabled);
    }

}
