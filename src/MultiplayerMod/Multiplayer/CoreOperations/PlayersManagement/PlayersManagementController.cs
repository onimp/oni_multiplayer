using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
using MultiplayerMod.Multiplayer.CoreOperations.PlayersManagement.Commands;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Multiplayer.World;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.CoreOperations.PlayersManagement;

[Dependency, UsedImplicitly]
public class PlayersManagementController {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<PlayersManagementController>();

    private readonly IMultiplayerServer server;
    private readonly IMultiplayerClient client;

    private readonly WorldManager worldManager;
    private readonly MultiplayerGame multiplayer;
    private readonly UnityTaskScheduler scheduler;

    private readonly IPlayerProfileProvider profileProvider;

    private readonly Dictionary<IMultiplayerClientId, PlayerIdentity> identities = new();

    public PlayersManagementController(
        IMultiplayerServer server,
        IMultiplayerClient client,
        EventDispatcher events,
        IPlayerProfileProvider profileProvider,
        WorldManager worldManager,
        MultiplayerGame multiplayer,
        UnityTaskScheduler scheduler
    ) {
        this.client = client;
        this.profileProvider = profileProvider;
        this.server = server;
        this.worldManager = worldManager;
        this.multiplayer = multiplayer;
        this.scheduler = scheduler;

        server.ClientDisconnected += OnClientDisconnected;
        events.Subscribe<ClientInitializationRequestEvent>(OnClientInitializationRequested);

        client.StateChanged += OnClientStateChanged;
        events.Subscribe<GameStartedEvent>(OnGameStarted);
        events.Subscribe<GameQuitEvent>(OnGameQuit);
        events.Subscribe<CurrentPlayerInitializedEvent>(OnCurrentPlayerInitialized);
        events.Subscribe<WorldSyncRequestedEvent>(OnWorldSaveRequested);
    }

    private void OnCurrentPlayerInitialized(CurrentPlayerInitializedEvent @event) {
        client.Send(new RequestPlayerStateChangeCommand(@event.Player.Id, PlayerState.Loading));
        if (@event.Player.Role == PlayerRole.Host)
            server.Send(new ChangePlayerStateCommand(@event.Player.Id, PlayerState.Ready));
        else
            client.Send(new RequestWorldSyncCommand());
    }

    private void OnClientStateChanged(MultiplayerClientState state) {
        if (state != MultiplayerClientState.Connected)
            return;

        client.Send(new InitializeClientCommand(profileProvider.GetPlayerProfile()));
    }

    private void OnGameStarted(GameStartedEvent @event) {
        if (@event.Multiplayer.Mode != MultiplayerMode.Client)
            return;

        var currentPlayer = @event.Multiplayer.Players.Current;
        scheduler.Run(() => client.Send(new RequestPlayerStateChangeCommand(currentPlayer.Id, PlayerState.Ready)));
    }

    private void OnGameQuit(GameQuitEvent @event) {
        client.Send(new RequestPlayerStateChangeCommand(multiplayer.Players.Current.Id, PlayerState.Leaving));
        client.Disconnect();
        multiplayer.Players.Synchronize(Array.Empty<MultiplayerPlayer>());
    }

    private void OnWorldSaveRequested(WorldSyncRequestedEvent _) {
        worldManager.Sync();
    }

    private void OnClientDisconnected(IMultiplayerClientId clientId) {
        if (!identities.TryGetValue(clientId, out var playerId))
            throw new PlayersManagementException($"No associated player found for client {clientId}");

        var player = multiplayer.Players[playerId];
        server.Send(new RemovePlayerCommand(player.Id));
        identities.Remove(clientId);
        log.Debug($"Client {clientId} disconnected {{ Id = {player.Id} }}");
    }

    private void OnClientInitializationRequested(ClientInitializationRequestEvent @event) {
        var host = @event.ClientId.Equals(client.Id);
        var role = host ? PlayerRole.Host : PlayerRole.Client;
        var player = new MultiplayerPlayer(role, @event.Profile);
        identities[@event.ClientId] = player.Id;

        server.Send(@event.ClientId, new SyncPlayersCommand(multiplayer.Players.ToArray()));
        server.Clients.ForEach(it => server.Send(it, new AddPlayerCommand(player, it.Equals(@event.ClientId))));

        log.Debug($"Client {@event.ClientId} initialized {{ Role = {role}, Id = {player.Id} }}");
    }

}
