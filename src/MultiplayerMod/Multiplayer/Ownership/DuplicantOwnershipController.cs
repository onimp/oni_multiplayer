using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.Game.Mechanics.Minions;
using MultiplayerMod.Multiplayer.Commands.Player;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Ownership;

/// <summary>
/// Host-side brain for per-player duplicant ownership. Owns the authoritative mutations (apply locally +
/// broadcast), the round-robin auto-split of duplicants across players, and re-pushing the full snapshot to
/// clients after every world sync (covers late joins and hard-syncs).
///
/// All mutating entry points assume they run on the host: the request commands that clients send
/// (<see cref="RequestAssignDuplicantOwner"/> / <see cref="RequestSetOwnershipFeatureEnabled"/>) execute on
/// the host, and the host's own UI calls these directly.
/// </summary>
[Dependency, UsedImplicitly]
public class DuplicantOwnershipController {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<DuplicantOwnershipController>();

    private readonly IMultiplayerServer server;
    private readonly DuplicantOwnershipRegistry registry;
    private readonly MultiplayerGame multiplayer;
    private readonly UnityTaskScheduler scheduler;

    // Round-robin cursor. Persists for the whole session so later-printed duplicants continue the rotation.
    // Starts at 0 and the ordered player list puts the host first, so an uneven leftover lands on the host.
    private int cursor;

    public DuplicantOwnershipController(
        IMultiplayerServer server,
        DuplicantOwnershipRegistry registry,
        MultiplayerGame multiplayer,
        UnityTaskScheduler scheduler,
        EventDispatcher events
    ) {
        this.server = server;
        this.registry = registry;
        this.multiplayer = multiplayer;
        this.scheduler = scheduler;

        events.Subscribe<GameReadyEvent>(_ => OnGameReady());
        events.Subscribe<WorldSavedEvent>(_ => ResyncClients());
        events.Subscribe<StopMultiplayerEvent>(_ => OnStop());
    }

    private void OnGameReady() {
        if (multiplayer.Mode != MultiplayerMode.Host)
            return;

        // Watch for later-printed duplicants. Remove-then-add keeps a single subscription even though
        // Components.LiveMinionIdentities is a fresh manager per loaded game.
        global::Components.LiveMinionIdentities.OnAdd -= OnMinionAdded;
        global::Components.LiveMinionIdentities.OnAdd += OnMinionAdded;

        // Assign the starting duplicants on the next tick - by then MultiplayerObjects'
        // SaveGameObjectsInitializer (also on GameReadyEvent) has registered their KPrefabId-based ids, so
        // GetMultiplayerInstance().Id resolves.
        scheduler.Run(ReconcileUnowned);
    }

    private void OnStop() {
        cursor = 0;
        global::Components.LiveMinionIdentities.OnAdd -= OnMinionAdded;
    }

    private void OnMinionAdded(MinionIdentity identity) {
        if (multiplayer.Mode != MultiplayerMode.Host)
            return;
        // Defer: at OnAdd time a printed duplicant's proxy id may not be registered yet.
        scheduler.Run(ReconcileUnowned);
    }

    /// <summary>
    /// Assigns every currently-unowned live duplicant to a player round-robin (host first). Idempotent -
    /// already-owned duplicants are skipped, so it is safe to call repeatedly.
    /// </summary>
    private void ReconcileUnowned() {
        if (multiplayer.Mode != MultiplayerMode.Host || !registry.Enabled)
            return;

        var players = OrderedPlayers();
        if (players.Count == 0)
            return;

        foreach (var identity in global::Components.LiveMinionIdentities.Items) {
            var proxyId = TryGetProxyId(identity);
            if (proxyId == null || registry.GetOwner(proxyId) != null)
                continue;

            var owner = players[cursor % players.Count].Id;
            cursor++;
            Assign(proxyId, owner);
            log.Debug($"Auto-assigned duplicant {identity.name} to {owner}");
        }
    }

    /// <summary>Host: applies an ownership change authoritatively and broadcasts it to clients.</summary>
    public void Assign(MultiplayerId proxyId, PlayerIdentity? owner) {
        registry.SetOwner(proxyId, owner);
        server.Send(new AssignDuplicantOwner(proxyId, owner));
    }

    /// <summary>Host: toggles the feature authoritatively and broadcasts it to clients.</summary>
    public void SetEnabled(bool enabled) {
        registry.SetEnabled(enabled);
        server.Send(new SetOwnershipFeatureEnabled(enabled));
        if (enabled)
            scheduler.Run(ReconcileUnowned);
    }

    private void ResyncClients() {
        if (multiplayer.Mode != MultiplayerMode.Host || server.Clients.Count == 0)
            return;

        var entries = registry.Snapshot()
            .Select(it => new DuplicantOwnershipEntry(it.Key, it.Value))
            .ToList();
        server.Send(new SyncDuplicantOwnership(entries, registry.Enabled));
    }

    private static MultiplayerId? TryGetProxyId(MinionIdentity identity) {
        try {
            return identity.GetMultiplayerInstance().Id;
        } catch {
            // Proxy not fully set up yet - it'll be caught on the next reconcile.
            return null;
        }
    }

    // Host first (so an uneven leftover goes to the host), then the remaining players in join order.
    private List<MultiplayerPlayer> OrderedPlayers() =>
        multiplayer.Players.OrderByDescending(player => player.Role == PlayerRole.Host).ToList();

}
