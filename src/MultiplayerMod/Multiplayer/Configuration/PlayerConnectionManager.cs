using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Game;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Multiplayer.Players.Commands;
using MultiplayerMod.Multiplayer.Players.Events;
using MultiplayerMod.Multiplayer.State;
using MultiplayerMod.Multiplayer.UI;
using MultiplayerMod.Multiplayer.World;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Configuration;

[UsedImplicitly]
public class PlayerConnectionManager {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<PlayerConnectionManager>();

    private readonly IMultiplayerServer server;
    private readonly IMultiplayerClient client;

    private readonly IPlayerProfileProvider profileProvider;
    private readonly WorldManager worldManager;
    private readonly EventDispatcher eventDispatcher;
    private readonly MultiplayerGame multiplayer;
    private readonly ExecutionLevelManager executionLevelManager;

    private readonly Dictionary<IMultiplayerClientId, PlayerIdentity> identities = new();

    public PlayerConnectionManager(
        IMultiplayerServer server,
        IMultiplayerClient client,
        IPlayerProfileProvider profileProvider,
        WorldManager worldManager,
        EventDispatcher eventDispatcher,
        MultiplayerGame multiplayer,
        ExecutionLevelManager executionLevelManager
    ) {
        this.server = server;
        this.client = client;
        this.profileProvider = profileProvider;
        this.worldManager = worldManager;
        this.eventDispatcher = eventDispatcher;
        this.multiplayer = multiplayer;
        this.executionLevelManager = executionLevelManager;

        GameEvents.GameStarted += OnGameStarted;
        client.StateChanged += OnClientStateChanged;
        server.ClientDisconnected += ServerOnClientDisconnected;
        eventDispatcher.Subscribe<ClientInitializationRequestEvent>(OnClientInitializationRequested);
        eventDispatcher.Subscribe<PlayerStateChangedEvent>(PlayerStateChanged);
    }

    public void LeaveGame() {
        eventDispatcher.Dispatch(new GameLeaveRequestedEvent());
        client.Send(new RequestPlayerStateChangeCommand(multiplayer.Players.Current.Id, PlayerState.Leaving));
    }

    private void PlayerStateChanged(PlayerStateChangedEvent @event) {
        if (@event.Player != multiplayer.Players.Current)
            return;

        if (@event.State != PlayerState.Leaving)
            return;

        client.Disconnect();
    }

    private void ServerOnClientDisconnected(IMultiplayerClientId clientId) {
        if (!identities.TryGetValue(clientId, out var playerId))
            throw new MultiplayerConnectionException($"No associated player found for client {clientId}");

        var player = multiplayer.Players[playerId];
        server.Send(new RemovePlayerCommand(player.Id));
        identities.Remove(clientId);
        log.Debug($"Client {client} disconnected");
    }

    private void OnClientStateChanged(MultiplayerClientState state) {
        switch (state) {
            case MultiplayerClientState.Connecting:
                if (multiplayer.Mode == MultiplayerMode.None)
                    multiplayer.Mode = MultiplayerMode.Client;
                if (multiplayer.Mode == MultiplayerMode.Client)
                    LoadOverlay.Show();
                break;
            case MultiplayerClientState.Connected:
                client.Send(
                    new InitializeClientCommand(profileProvider.GetPlayerProfile()),
                    MultiplayerCommandOptions.ExecuteOnServer
                );
                break;
            case MultiplayerClientState.Disconnected:
                multiplayer.Players.Synchronize(Array.Empty<MultiplayerPlayer>());
                break;
        }
    }

    private void OnGameStarted() {
        if (multiplayer.Mode != MultiplayerMode.Client)
            return;

        var currentPlayer = multiplayer.Players.Current;
        var command = new RequestPlayerStateChangeCommand(currentPlayer.Id, PlayerState.Ready);
        client.Send(command, MultiplayerCommandOptions.ExecuteOnServer);

        executionLevelManager.ReplaceLevel(ExecutionLevel.Gameplay);
    }

    private void OnClientInitializationRequested(ClientInitializationRequestEvent @event) {
        var host = @event.ClientId.Equals(client.Id);
        var role = host ? PlayerRole.Host : PlayerRole.Client;
        var player = new MultiplayerPlayer(role, @event.Profile);
        identities[@event.ClientId] = player.Id;

        server.Send(@event.ClientId, new SyncPlayersCommand(multiplayer.Players.ToArray()));
        server.Clients.ForEach(
            clientId => server.Send(clientId, new AddPlayerCommand(player, clientId.Equals(@event.ClientId)))
        );

        if (host) {
            log.Debug($"Host {@event.ClientId} initialized");
            server.Send(new ChangePlayerStateCommand(player.Id, PlayerState.Ready));
            return;
        }

        log.Debug($"Client {@event.ClientId} initialized");
        worldManager.Sync();
    }

}
