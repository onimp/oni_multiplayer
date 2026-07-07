using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Game.Mechanics.Minions;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Player;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Ownership;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.UI.Ownership;

/// <summary>
/// Adds per-player assignment controls to a duplicant's right-click (user) menu on every machine:
/// a "MP: {owner}" button that cycles the duplicant through the players (and Unassigned) on click, plus a
/// host-only button to toggle the whole feature. This is the interim UI while a dedicated top-bar management
/// screen is built - it reuses ONI's own user-menu affordance so it needs no custom prefab.
/// </summary>
[Dependency, UsedImplicitly]
public class DuplicantOwnershipUserMenu {

    private readonly DuplicantOwnershipRegistry registry;
    private readonly DuplicantOwnershipController controller;
    private readonly MultiplayerGame multiplayer;
    private readonly IMultiplayerClient client;
    private readonly ExecutionLevelManager executionLevel;
    private readonly EventDispatcher events;

    // Minions we've already wired a RefreshUserMenu handler onto, so re-scanning existing duplicants (client
    // after hard-sync) doesn't double-subscribe.
    private readonly HashSet<int> subscribed = new();

    public DuplicantOwnershipUserMenu(
        DuplicantOwnershipRegistry registry,
        DuplicantOwnershipController controller,
        MultiplayerGame multiplayer,
        IMultiplayerClient client,
        ExecutionLevelManager executionLevel,
        EventDispatcher events
    ) {
        this.registry = registry;
        this.controller = controller;
        this.multiplayer = multiplayer;
        this.client = client;
        this.executionLevel = executionLevel;
        this.events = events;

        events.Subscribe<GameReadyEvent>(_ => OnGameReady());
        events.Subscribe<StopMultiplayerEvent>(_ => OnStop());
        // Live-refresh the open menu when ownership/enabled changes so labels update without reopening.
        events.Subscribe<DuplicantOwnershipChangedEvent>(_ => RefreshOpenMenu());
    }

    private void OnGameReady() {
        global::Components.LiveMinionIdentities.OnAdd -= OnMinionAdded;
        global::Components.LiveMinionIdentities.OnAdd += OnMinionAdded;
        // Duplicants that already exist (e.g. a client whose world arrived via hard-sync before this fired).
        foreach (var identity in global::Components.LiveMinionIdentities.Items.ToList())
            OnMinionAdded(identity);
    }

    private void OnStop() {
        global::Components.LiveMinionIdentities.OnAdd -= OnMinionAdded;
        subscribed.Clear();
    }

    private void OnMinionAdded(MinionIdentity identity) {
        if (identity == null || !subscribed.Add(identity.GetInstanceID()))
            return;
        identity.gameObject.Subscribe((int) GameHashes.RefreshUserMenu, _ => OnRefreshUserMenu(identity));
    }

    private void OnRefreshUserMenu(MinionIdentity identity) {
        if (global::Game.Instance == null || !executionLevel.LevelIsActive(ExecutionLevel.Multiplayer))
            return;

        var proxyId = TryGetProxyId(identity);
        if (proxyId == null)
            return;

        if (registry.Enabled) {
            var owner = registry.GetOwner(proxyId);
            global::Game.Instance.userMenu.AddButton(
                identity.gameObject,
                new KIconButtonMenu.ButtonInfo(
                    "action_control",
                    $"MP: {OwnerName(owner)}",
                    () => CycleOwner(proxyId, owner),
                    tooltipText: "Multiplayer owner. Click to assign this duplicant to the next player."
                ),
                1f
            );
        }

        // Host-only global toggle. Lives on the duplicant menu for now; will move to the top-bar screen later.
        if (multiplayer.Mode == MultiplayerMode.Host) {
            var enabled = registry.Enabled;
            global::Game.Instance.userMenu.AddButton(
                identity.gameObject,
                new KIconButtonMenu.ButtonInfo(
                    "action_control",
                    enabled ? "MP: Disable assignment" : "MP: Enable assignment",
                    () => controller.SetEnabled(!enabled),
                    tooltipText: "Host only: turn per-player duplicant assignment on or off for everyone."
                ),
                0.5f
            );
        }
    }

    private void CycleOwner(MultiplayerId proxyId, PlayerIdentity? current) {
        var options = multiplayer.Players
            .OrderByDescending(player => player.Role == PlayerRole.Host)
            .Select(player => (PlayerIdentity?) player.Id)
            .ToList();
        options.Add(null); // Unassigned

        var index = options.FindIndex(option => Equals(option, current));
        if (index < 0)
            index = options.Count - 1;
        var next = options[(index + 1) % options.Count];

        if (multiplayer.Mode == MultiplayerMode.Host)
            controller.Assign(proxyId, next);
        else
            client.Send(new RequestAssignDuplicantOwner(proxyId, next));
    }

    private string OwnerName(PlayerIdentity? owner) {
        if (owner == null)
            return "Unassigned";
        var player = multiplayer.Players.FirstOrDefault(it => it.Id.Equals(owner));
        return player?.Profile.PlayerName ?? "Unknown";
    }

    private static void RefreshOpenMenu() {
        if (global::Game.Instance == null)
            return;
        var selected = SelectTool.Instance?.selected;
        if (selected != null)
            selected.gameObject.Trigger((int) GameHashes.RefreshUserMenu);
    }

    private static MultiplayerId? TryGetProxyId(MinionIdentity identity) {
        try {
            return identity.GetMultiplayerInstance().Id;
        } catch {
            return null;
        }
    }

}
