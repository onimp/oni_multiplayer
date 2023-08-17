using System;
using System.Linq;
using JetBrains.Annotations;
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

[UsedImplicitly]
public class MultiplayerCoordinator {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<MultiplayerCoordinator>();

    private readonly IMultiplayerServer server;
    private readonly IMultiplayerClient client;

    private readonly GameEventBindings gameBindings;
    private readonly ServerEventBindings serverBindings;

    private readonly MultiplayerGame multiplayer;

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
        MultiplayerGame multiplayer
    ) {
        this.server = server;
        this.client = client;
        this.gameBindings = gameBindings;
        this.serverBindings = serverBindings;
        this.multiplayer = multiplayer;

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
        if (!multiplayer.State.Players.ContainsKey(player)) {
            multiplayer.State.Players.Add(player, new PlayerState(player));
        }
        if (player.Equals(client.Player))
            return;

        log.Debug($"Player {player} connected");
        WorldManager.Sync();
    }

    private void OnPlayerDisconnected(IPlayerIdentity player) {
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
                    multiplayer.Role = MultiplayerRole.Client;
                    multiplayer.State.Players.Add(client.Player, new PlayerState(client.Player));
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

    private void ClientOnCommandReceived(CommandReceivedEventArgs e) {
        if (!multiplayer.State.Current.WorldSpawned && !unSpawnedWorldCommands.Contains(e.Command.GetType())) {
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
        if (multiplayer.Role == MultiplayerRole.None)
            return;

        PatchContext.Global = PatchControl.EnablePatches;

        if (multiplayer.Role == MultiplayerRole.Host) {
            LoadOverlay.Show();
            server.Start();
            multiplayer.State.Players.Add(client.Player, new PlayerState(client.Player));
        }
        if (multiplayer.Role == MultiplayerRole.Client) {
            client.Send(
                new MultiplayerEvents.PlayerWorldSpawnedEvent(client.Player),
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
