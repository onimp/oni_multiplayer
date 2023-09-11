using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Game;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Components;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Multiplayer.Players.Events;
using MultiplayerMod.Multiplayer.UI;
using MultiplayerMod.Multiplayer.World.Debug;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Configuration;

[UsedImplicitly]
public class MultiplayerCoordinator {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<MultiplayerCoordinator>();

    private readonly IMultiplayerServer server;
    private readonly IMultiplayerClient client;

    private readonly MultiplayerGame multiplayer;
    private readonly ExecutionLevelManager executionLevelManager;
    private readonly EventDispatcher eventDispatcher;
    private readonly MultiplayerCommandExecutor commandExecutor;
    private readonly DependencyContainer dependencies;

    public MultiplayerCoordinator(
        IMultiplayerServer server,
        IMultiplayerClient client,
        MultiplayerGame multiplayer,
        ExecutionLevelManager executionLevelManager,
        EventDispatcher eventDispatcher,
        MultiplayerCommandExecutor commandExecutor,
        DependencyContainer dependencies
    ) {
        this.server = server;
        this.client = client;
        this.multiplayer = multiplayer;
        this.executionLevelManager = executionLevelManager;
        this.eventDispatcher = eventDispatcher;
        this.commandExecutor = commandExecutor;
        this.dependencies = dependencies;

        ConfigureServer();
        ConfigureClient();
        GameEvents.GameStarted += OnGameStarted;
        eventDispatcher.Subscribe<MultiplayerConnectRequestedEvent>(OnMultiplayerConnectRequested);
    }

    private void OnMultiplayerConnectRequested(MultiplayerConnectRequestedEvent @event) {
        multiplayer.Refresh(MultiplayerMode.Client);
        client.Connect(@event.Endpoint);
        LoadOverlay.Show($"Connecting to {@event.Name}...");
    }

    #region Server configuration

    private void ConfigureServer() {
        server.StateChanged += OnServerStateChanged;
        server.CommandReceived += ServerOnCommandReceived;
        dependencies.Resolve<ServerEventBindings>().Bind();
        eventDispatcher.Subscribe<PlayerStateChangedEvent>(OnPlayerStateChanged);
    }

    private void OnPlayerStateChanged(PlayerStateChangedEvent @event) {
        if (multiplayer.Players.Current != @event.Player)
            return;
        if (multiplayer.Players.Current.State == PlayerState.Ready)
            executionLevelManager.BaseLevel = ExecutionLevel.Game;
    }

    private void OnServerStateChanged(MultiplayerServerState state) {
        log.Debug($"Server state changed: {state}");
        switch (state) {
            case MultiplayerServerState.Started:
                client.Connect(server.Endpoint);
                break;
        }
    }

    private void ServerOnCommandReceived(IMultiplayerClientId clientId, IMultiplayerCommand command) {
        log.Trace(() => $"{command} received from {clientId}");
        commandExecutor.Execute(clientId, command);
    }

    #endregion

    #region Client configuration

    private void ConfigureClient() {
        client.StateChanged += OnClientStateChanged;
        client.CommandReceived += ClientOnCommandReceived;
        dependencies.Resolve<GameEventBindings>().Bind();
    }

    private void OnClientStateChanged(MultiplayerClientState state) {
        switch (state) {
            case MultiplayerClientState.Connecting:
                if (multiplayer.Mode == MultiplayerMode.Client)
                    executionLevelManager.BaseLevel = ExecutionLevel.Multiplayer;
                break;
        }
    }

    private void ClientOnCommandReceived(IMultiplayerCommand command) {
        log.Trace(() => $"{command} received from server");
        commandExecutor.Execute(null, command);
    }

    #endregion

    private void OnGameStarted() {
        if (multiplayer.Mode == MultiplayerMode.Disabled)
            return;

        if (multiplayer.Mode == MultiplayerMode.Host) {
            LoadOverlay.Show("Starting host...");
            server.Start();
        }

        UnityObject.CreateWithComponent<
            DrawCursorComponent,
            WorldDebugSnapshotRunner
        >();
    }

}
