using System;
using System.Linq;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Core.Patch.Context;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Game;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Commands.Overlay;
using MultiplayerMod.Multiplayer.Commands.State;
using MultiplayerMod.Multiplayer.Components;
using MultiplayerMod.Multiplayer.State;
using MultiplayerMod.Multiplayer.UI;
using MultiplayerMod.Multiplayer.World;
using MultiplayerMod.Multiplayer.World.Debug;
using MultiplayerMod.Network;
using MultiplayerMod.Network.Events;

namespace MultiplayerMod.Multiplayer.Configuration;

public class MultiplayerCoordinator {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<MultiplayerCoordinator>();

    private readonly IMultiplayerServer server;
    private readonly IMultiplayerClient client;

    private readonly GameEventBindings gameBindings;
    private readonly ServerEventBindings serverBindings;

    private readonly CommandExceptionHandler exceptionHandler = new();

    private readonly Type[] unSpawnedWorldCommands = {
        typeof(LoadWorld),
        typeof(ShowLoadOverlay)
    };

    public MultiplayerCoordinator(
        IMultiplayerServer server,
        IMultiplayerClient client,
        GameEventBindings gameBindings,
        ServerEventBindings serverBindings
    ) {
        this.server = server;
        this.client = client;
        this.gameBindings = gameBindings;
        this.serverBindings = serverBindings;

        ConfigureServer();
        ConfigureClient();
        GameEvents.GameStarted += OnGameStarted;
    }

    #region Server configuration

    private void ConfigureServer() {
        server.StateChanged += OnServerStateChanged;
        server.PlayerConnected += OnPlayerConnected;
        server.PlayerDisconnected += OnPlayerDisconnected;
        server.CommandReceived += ServerOnCommandReceived;
    }

    private void OnServerStateChanged(ServerStateChangedEventArgs e) {
        log.Debug($"Server state changed: {e.State}");
        switch (e.State) {
            case MultiplayerServerState.Starting:
                serverBindings.Bind();
                break;
            case MultiplayerServerState.Started:
                client.Connect(server.Endpoint);
                break;
        }
    }

    private void ServerOnCommandReceived(CommandReceivedEventArgs e) {
        log.Trace(() => $"{e.Command} received from player {e.Player}");
        PatchControl.RunWithDisabledPatches(() => ExecuteCommand(e.Command));
    }

    private void OnPlayerConnected(IPlayerIdentity player) {
        if (!MultiplayerGame.State.Players.ContainsKey(player)) {
            MultiplayerGame.State.Players.Add(player, new PlayerState(player));
        }
        if (player.Equals(client.Player))
            return;

        log.Debug($"Player {player} connected");
        WorldManager.Sync();
    }

    private void OnPlayerDisconnected(IPlayerIdentity player) {
        MultiplayerGame.State.Players.Remove(player);
        server.Send(new SyncMultiplayerState(MultiplayerGame.State));
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
                if (MultiplayerGame.Role != MultiplayerRole.Host) {
                    MultiplayerGame.Role = MultiplayerRole.Client;
                    MultiplayerGame.State.Players.Add(client.Player, new PlayerState(client.Player));
                    LoadOverlay.Show();
                }
                break;
            case MultiplayerClientState.Connected:
                gameBindings.Bind();
                if (MultiplayerGame.Role == MultiplayerRole.Host) {
                    // Server initialization happens after world spawn. So world is ready only after server has initialized.
                    MultiplayerGame.State.Current.WorldSpawned = true;
                }
                break;
        }
    }

    private void ClientOnCommandReceived(CommandReceivedEventArgs e) {
        if (!MultiplayerGame.State.Current.WorldSpawned && !unSpawnedWorldCommands.Contains(e.Command.GetType())) {
            log.Warning($"{e.Command} received, but the world isn't spawned yet");
            return;
        }
        log.Trace(() => $"{e.Command} received from server");
        PatchControl.RunWithDisabledPatches(() => ExecuteCommand(e.Command));
    }

    #endregion

    private void ExecuteCommand(IMultiplayerCommand command) {
        try {
            command.Execute();
        } catch (Exception exception) {
            exceptionHandler.Handle(command, exception);
        }
    }

    private void OnGameStarted() {
        if (MultiplayerGame.Role == MultiplayerRole.None)
            return;

        PatchContext.Global = PatchControl.EnablePatches;

        if (MultiplayerGame.Role == MultiplayerRole.Host) {
            LoadOverlay.Show();
            server.Start();
            MultiplayerGame.State.Players.Add(client.Player, new PlayerState(client.Player));
        }
        if (MultiplayerGame.Role == MultiplayerRole.Client) {
            client.Send(
                new MultiplayerEvents.PlayerWorldSpawnedEvent(client.Player),
                MultiplayerCommandOptions.ExecuteOnServer
            );
            MultiplayerGame.State.Current.WorldSpawned = true;
        }

        UnityObject.CreateWithComponent<
            DrawCursorComponent,
            WorldDebugSnapshotRunner
        >();
    }
}
