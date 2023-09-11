using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Game;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Components;
using MultiplayerMod.Multiplayer.CoreOperations.CommandExecution;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
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
    private readonly MultiplayerCommandExecutor commandExecutor;
    private readonly DependencyContainer dependencies;

    public MultiplayerCoordinator(
        IMultiplayerServer server,
        IMultiplayerClient client,
        MultiplayerGame multiplayer,
        EventDispatcher eventDispatcher,
        MultiplayerCommandExecutor commandExecutor,
        DependencyContainer dependencies
    ) {
        this.server = server;
        this.client = client;
        this.multiplayer = multiplayer;
        this.commandExecutor = commandExecutor;
        this.dependencies = dependencies;

        ConfigureServer();
        ConfigureClient();
        GameEvents.GameStarted += OnGameStarted;
        eventDispatcher.Subscribe<MultiplayerConnectRequestedEvent>(OnMultiplayerConnectRequested);
        eventDispatcher.Subscribe<MultiplayerGameQuittingEvent>(OnMultiplayerQuittingEvent);
    }

    private void OnMultiplayerQuittingEvent(MultiplayerGameQuittingEvent @event) {
        if (@event.Multiplayer.Mode == MultiplayerMode.Host)
            server.Stop();
    }

    private void OnMultiplayerConnectRequested(MultiplayerConnectRequestedEvent @event) {
        multiplayer.Refresh(MultiplayerMode.Client);
        LoadOverlay.Show($"Connecting to {@event.Name}...");
        client.Connect(@event.Endpoint);
    }

    #region Server configuration

    private void ConfigureServer() {
        server.StateChanged += OnServerStateChanged;
        server.CommandReceived += ServerOnCommandReceived;
        dependencies.Resolve<ServerEventBindings>().Bind();
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
        client.CommandReceived += ClientOnCommandReceived;
        dependencies.Resolve<GameEventBindings>().Bind();
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
