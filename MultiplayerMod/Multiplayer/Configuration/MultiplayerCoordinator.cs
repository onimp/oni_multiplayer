using System;
using System.Linq;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Game.World;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Commands.Overlay;
using MultiplayerMod.Multiplayer.Commands.State;
using MultiplayerMod.Multiplayer.Components;
using MultiplayerMod.Multiplayer.State;
using MultiplayerMod.Multiplayer.World;
using MultiplayerMod.Multiplayer.World.Debug;
using MultiplayerMod.Network;
using MultiplayerMod.Network.Events;

namespace MultiplayerMod.Multiplayer.Configuration;

public class MultiplayerCoordinator {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<MultiplayerCoordinator>();

    private readonly IMultiplayerServer server = Container.Get<IMultiplayerServer>();
    private readonly IMultiplayerClient client = Container.Get<IMultiplayerClient>();

    private readonly GameEventBindings gameBindings = new();
    private readonly ServerEventBindings serverBindings = new();

    private readonly CommandExceptionHandler exceptionHandler = new();

    private readonly Type[] unSpawnedWorldCommands = {
        typeof(LoadWorld),
        typeof(ShowLoadOverlay)
    };

    public MultiplayerCoordinator() {
        ConfigureServer();
        ConfigureClient();
        WorldGenSpawnerEvents.Spawned += OnWorldSpawned;
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
        log.Trace(() => $"Command {e.Command.GetType().Name} received from player {e.Player}");
        PatchControl.RunWithDisabledPatches(() => ExecuteCommand(e.Command));
    }

    private void OnPlayerConnected(PlayerEventArgs e) {
        if (!MultiplayerGame.State.Players.ContainsKey(e.Player)) {
            MultiplayerGame.State.Players.Add(e.Player, new PlayerState(e.Player));
        }
        if (e.Player.Equals(client.Player))
            return;

        log.Debug($"Player {e.Player} connected");
        WorldManager.Sync();
    }

    private void OnPlayerDisconnected(PlayerEventArgs e) {
        MultiplayerGame.State.Players.Remove(e.Player);
        server.Send(new SyncMultiplayerState(MultiplayerGame.State));
        log.Debug($"Player {e.Player} disconnected");
    }

    #endregion

    #region Client configuration

    private void ConfigureClient() {
        client.StateChanged += OnClientStateChanged;
        client.CommandReceived += ClientOnCommandReceived;
    }

    private void OnClientStateChanged(ClientStateChangedEventArgs e) {
        switch (e.State) {
            case MultiplayerClientState.Connecting:
                if (MultiplayerGame.Role != MultiplayerRole.Host) {
                    MultiplayerGame.Role = MultiplayerRole.Client;
                    ShowLoadOverlay.Show();
                }
                MultiplayerGame.Player = client.Player;
                break;
            case MultiplayerClientState.Connected:
                gameBindings.Bind();
                break;
        }
    }

    private void ClientOnCommandReceived(CommandReceivedEventArgs e) {
        var commandType = e.Command.GetType();
        if (!MultiplayerGame.CurrentPlayerState.Spawned && !unSpawnedWorldCommands.Contains(commandType)) {
            log.Warning($"Command {commandType.FullName} received, but the world isn't spawned yet");
            return;
        }
        log.Trace(() => $"Command {commandType.Name} received from server");
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

    private void OnWorldSpawned() {
        switch (MultiplayerGame.Role) {
            case MultiplayerRole.None:
                return;
            case MultiplayerRole.Host:
                server.Start();
                break;
            case MultiplayerRole.Client:
                client.Send(
                    new MultiplayerEvents.PlayerWorldSpawnedEvent(client.Player),
                    MultiplayerCommandOptions.ExecuteOnServer
                );
                break;
        }

        UnityObject.CreateWithComponent<
            DrawCursorComponent,
            WorldDebugSnapshotRunner
        >();

        MultiplayerGame.Player = client.Player;
        MultiplayerGame.CurrentPlayerState.Spawned = true;
    }
}
