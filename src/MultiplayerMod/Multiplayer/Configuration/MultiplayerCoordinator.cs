using System;
using JetBrains.Annotations;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Game;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Components;
using MultiplayerMod.Multiplayer.Players.Events;
using MultiplayerMod.Multiplayer.State;
using MultiplayerMod.Multiplayer.UI;
using MultiplayerMod.Multiplayer.World.Debug;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Configuration;

[UsedImplicitly]
public class MultiplayerCoordinator {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<MultiplayerCoordinator>();

    private readonly IMultiplayerServer server;
    private readonly IMultiplayerClient client;

    private readonly GameEventBindings gameBindings;
    private readonly ServerEventBindings serverBindings;

    private readonly MultiplayerGame multiplayer;
    private readonly ExecutionLevelManager executionLevelManager;
    private readonly Runtime runtime;
    private readonly EventDispatcher eventDispatcher;
    private readonly MultiplayerCommandExecutor commandExecutor;

    public MultiplayerCoordinator(
        IMultiplayerServer server,
        IMultiplayerClient client,
        GameEventBindings gameBindings,
        ServerEventBindings serverBindings,
        MultiplayerGame multiplayer,
        ExecutionLevelManager executionLevelManager,
        Runtime runtime,
        EventDispatcher eventDispatcher,
        MultiplayerCommandExecutor commandExecutor
    ) {
        this.server = server;
        this.client = client;
        this.gameBindings = gameBindings;
        this.serverBindings = serverBindings;
        this.multiplayer = multiplayer;
        this.executionLevelManager = executionLevelManager;
        this.runtime = runtime;
        this.eventDispatcher = eventDispatcher;
        this.commandExecutor = commandExecutor;

        ConfigureServer();
        ConfigureClient();
        GameEvents.GameStarted += OnGameStarted;
    }

    #region Server configuration

    private void ConfigureServer() {
        server.StateChanged += OnServerStateChanged;
        server.CommandReceived += ServerOnCommandReceived;
        serverBindings.Bind();
        eventDispatcher.Subscribe<PlayerJoinedEvent>(OnPlayerJoined);
    }

    private void OnPlayerJoined(PlayerJoinedEvent @event) {
        if (multiplayer.Players.Current != @event.Player)
            return;
        runtime.Dependencies.Get<UnityTaskScheduler>().Run(() => {
            executionLevelManager.ReplaceLevel(ExecutionLevel.Gameplay);
        });
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
        gameBindings.Bind();
    }

    private void OnClientStateChanged(MultiplayerClientState state) {
        switch (state) {
            case MultiplayerClientState.Connecting:
                if (multiplayer.Mode == MultiplayerMode.Client)
                    executionLevelManager.ReplaceLevel(ExecutionLevel.Multiplayer);
                break;
        }
    }

    private void ClientOnCommandReceived(IMultiplayerCommand command) {
        log.Trace(() => $"{command} received from server");
        commandExecutor.Execute(null, command);
    }

    #endregion

    private void OnGameStarted() {
        if (multiplayer.Mode == MultiplayerMode.None)
            return;

        if (multiplayer.Mode == MultiplayerMode.Host) {
            LoadOverlay.Show();
            server.Start();
        }

        UnityObject.CreateWithComponent<
            DrawCursorComponent,
            WorldDebugSnapshotRunner
        >();
    }

}
