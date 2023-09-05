using System;
using System.Linq;
using JetBrains.Annotations;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Game;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Commands.Overlay;
using MultiplayerMod.Multiplayer.Commands.State;
using MultiplayerMod.Multiplayer.Components;
using MultiplayerMod.Multiplayer.State;
using MultiplayerMod.Multiplayer.UI;
using MultiplayerMod.Multiplayer.World;
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

    private readonly CommandExceptionHandler exceptionHandler = new();

    private readonly Type[] unSpawnedWorldCommands = {
        typeof(LoadWorld),
        typeof(ShowLoadOverlay)
    };

    public MultiplayerCoordinator(
        IMultiplayerServer server,
        IMultiplayerClient client,
        GameEventBindings gameBindings,
        ServerEventBindings serverBindings,
        MultiplayerGame multiplayer,
        ExecutionLevelManager executionLevelManager,
        Runtime runtime
    ) {
        this.server = server;
        this.client = client;
        this.gameBindings = gameBindings;
        this.serverBindings = serverBindings;
        this.multiplayer = multiplayer;
        this.executionLevelManager = executionLevelManager;
        this.runtime = runtime;

        ConfigureServer();
        ConfigureClient();
        GameEvents.GameStarted += OnGameStarted;
    }

    #region Server configuration

    private void ConfigureServer() {
        server.StateChanged += OnServerStateChanged;
        server.ClientConnected += OnClientConnected;
        server.ClientDisconnected += OnClientDisconnected;
        server.CommandReceived += ServerOnCommandReceived;
    }

    private void OnServerStateChanged(MultiplayerServerState state) {
        log.Debug($"Server state changed: {state}");
        switch (state) {
            case MultiplayerServerState.Starting:
                serverBindings.Bind();
                break;
            case MultiplayerServerState.Started:
                client.Connect(server.Endpoint);
                break;
        }
    }

    private void ServerOnCommandReceived(IMultiplayerClientId clientId, IMultiplayerCommand command) {
        log.Trace(() => $"{command} received from {clientId}");
        executionLevelManager.RunUsingLevel(ExecutionLevel.Command, () => ExecuteCommand(clientId, command));
    }

    private void OnClientConnected(IMultiplayerClientId player) {
        if (!multiplayer.State.Players.ContainsKey(player)) {
            multiplayer.State.Players.Add(player, new PlayerState(player));
        }
        if (player.Equals(client.Id))
            return;

        log.Debug($"Player {player} connected");
        WorldManager.Sync();
    }

    private void OnClientDisconnected(IMultiplayerClientId player) {
        multiplayer.State.Players.Remove(player);
        server.Send(new SyncMultiplayerState(multiplayer.State));
        log.Debug($"Player {player} disconnected");
    }

    #endregion

    #region Client configuration

    private void ConfigureClient() {
        client.StateChanged += OnClientStateChanged;
        client.CommandReceived += ClientOnCommandReceived;
    }

    private void OnClientStateChanged(MultiplayerClientState state) {
        switch (state) {
            case MultiplayerClientState.Connecting:
                if (multiplayer.Role != MultiplayerRole.Host) {
                    executionLevelManager.ReplaceLevel(ExecutionLevel.Multiplayer);
                    multiplayer.Role = MultiplayerRole.Client;
                    multiplayer.State.Players.Add(client.Id, new PlayerState(client.Id));
                    LoadOverlay.Show();
                }
                break;
            case MultiplayerClientState.Connected:
                gameBindings.Bind();
                if (multiplayer.Role == MultiplayerRole.Host) {
                    // Server initialization happens after world spawn. So world is ready only after server has initialized.
                    multiplayer.State.Current.WorldSpawned = true;
                }
                break;
        }
    }

    private void ClientOnCommandReceived(IMultiplayerCommand command) {
        if (!multiplayer.State.Current.WorldSpawned && !unSpawnedWorldCommands.Contains(command.GetType())) {
            log.Warning($"{command} received, but the world isn't spawned yet");
            return;
        }
        log.Trace(() => $"{command} received from server");
        executionLevelManager.RunUsingLevel(ExecutionLevel.Command, () => ExecuteCommand(null, command));
    }

    #endregion

    private void ExecuteCommand(IMultiplayerClientId? clientId, IMultiplayerCommand command) {
        try {
            command.Execute(new MultiplayerCommandContext(clientId, runtime));
        } catch (Exception exception) {
            exceptionHandler.Handle(command, exception);
        }
    }

    private void OnGameStarted() {
        if (multiplayer.Role == MultiplayerRole.None)
            return;

        executionLevelManager.ReplaceLevel(ExecutionLevel.Gameplay);

        if (multiplayer.Role == MultiplayerRole.Host) {
            LoadOverlay.Show();
            server.Start();
            multiplayer.State.Players.Add(client.Id, new PlayerState(client.Id));
        }
        if (multiplayer.Role == MultiplayerRole.Client) {
            client.Send(
                new MultiplayerEvents.PlayerWorldSpawnedEvent(client.Id),
                MultiplayerCommandOptions.ExecuteOnServer
            );
            multiplayer.State.Current.WorldSpawned = true;
        }

        UnityObject.CreateWithComponent<
            DrawCursorComponent,
            WorldDebugSnapshotRunner
        >();
    }
}
