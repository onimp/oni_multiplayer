using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Players;

namespace MultiplayerMod.Multiplayer.Ownership;

/// <summary>
/// Maps a duplicant (keyed by its assignable-proxy <see cref="MultiplayerId"/>) to the player who owns it.
/// The host is the source of truth; clients hold a mirror kept up to date by <c>AssignDuplicantOwner</c> /
/// <c>SyncDuplicantOwnership</c>. Ownership only influences chore selection on the host (Phase 2); the mirror
/// on clients exists purely so their management UI can display who owns whom.
///
/// This is a display/organizational layer - it does NOT change chore behaviour on its own. The host-side
/// preference precondition (Phase 2) reads this registry to bias assignment.
/// </summary>
[Dependency, UsedImplicitly]
public class DuplicantOwnershipRegistry {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<DuplicantOwnershipRegistry>();

    private readonly EventDispatcher events;

    private readonly Dictionary<MultiplayerId, PlayerIdentity> owners = new();

    /// <summary>
    /// Whether per-player assignment is active. When false the whole feature is inert: no auto-split, and the
    /// Phase 2 preference precondition passes everything (vanilla behaviour). Host-authoritative, synced via
    /// <c>SetOwnershipFeatureEnabled</c>.
    /// </summary>
    public bool Enabled { get; private set; } = true;

    public DuplicantOwnershipRegistry(EventDispatcher events) {
        this.events = events;
        events.Subscribe<GameQuitEvent>(_ => Clear());
        events.Subscribe<StopMultiplayerEvent>(_ => Clear());
    }

    public PlayerIdentity? GetOwner(MultiplayerId id) => owners.TryGetValue(id, out var owner) ? owner : null;

    /// <summary>Sets (or, with a null owner, clears) the owner of a duplicant and notifies UI.</summary>
    public void SetOwner(MultiplayerId id, PlayerIdentity? owner) {
        if (owner == null)
            owners.Remove(id);
        else
            owners[id] = owner;
        events.Dispatch(new DuplicantOwnershipChangedEvent());
    }

    /// <summary>Replaces the entire ownership map from an authoritative host snapshot (join / hard-sync).</summary>
    public void Replace(IEnumerable<KeyValuePair<MultiplayerId, PlayerIdentity>> snapshot) {
        owners.Clear();
        foreach (var pair in snapshot)
            owners[pair.Key] = pair.Value;
        events.Dispatch(new DuplicantOwnershipChangedEvent());
    }

    public void SetEnabled(bool enabled) {
        if (Enabled == enabled)
            return;
        Enabled = enabled;
        log.Debug($"Duplicant ownership feature {(enabled ? "enabled" : "disabled")}");
        events.Dispatch(new DuplicantOwnershipChangedEvent());
    }

    /// <summary>A stable snapshot of the current ownership map, for serialization into a sync command.</summary>
    public List<KeyValuePair<MultiplayerId, PlayerIdentity>> Snapshot() => owners.ToList();

    /// <summary>Number of duplicants currently owned by the given player (host-side balancing).</summary>
    public int CountOwnedBy(PlayerIdentity player) => owners.Values.Count(it => it.Equals(player));

    private void Clear() {
        owners.Clear();
        Enabled = true;
    }

}
