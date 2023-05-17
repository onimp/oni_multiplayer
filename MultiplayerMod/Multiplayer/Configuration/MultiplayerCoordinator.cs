using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Game.World;
using MultiplayerMod.Multiplayer.Commands;
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

    public MultiplayerCoordinator() {
        ConfigureServer();
        ConfigureClient();
        WorldGenSpawnerEvents.Spawned += OnWorldSpawned;
    }

    #region Server configuration

    private void ConfigureServer() {
        server.StateChanged += OnServerStateChanged;
        server.PlayerConnected += OnPlayerConnected;
        server.CommandReceived += ServerOnCommandReceived;
    }

    private void OnServerStateChanged(object sender, ServerStateChangedEventArgs e) {
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

    private void ServerOnCommandReceived(object sender, CommandReceivedEventArgs e) {
        log.Trace(() => $"Command {e.Command.GetType().Name} received from player {e.Player}");
        PatchControl.RunWithDisabledPatches(() => e.Command.Execute());
    }

    private void OnPlayerConnected(object sender, PlayerConnectedEventArgs e) {
        MultiplayerState.Shared.Players[e.Player] = new PlayerSharedState(e.Player);
        if (e.Player.Equals(client.Player))
            return;

        log.Debug($"Player {e.Player} connected");
        WorldManager.Sync();
    }

    #endregion

    #region Client configuration

    private void ConfigureClient() {
        client.StateChanged += OnClientStateChanged;
        client.CommandReceived += ClientOnCommandReceived;
    }

    private void OnClientStateChanged(object sender, ClientStateChangedEventArgs e) {
        switch (e.State) {
            case MultiplayerClientState.Connecting:
                if (MultiplayerState.Role != MultiplayerRole.Host) {
                    MultiplayerState.Role = MultiplayerRole.Client;
                    LoadingOverlay.Load(() => { });
                }
                break;
            case MultiplayerClientState.Connected:
                gameBindings.Bind();
                break;
        }
    }

    private void ClientOnCommandReceived(object sender, CommandReceivedEventArgs e) {
        if (!MultiplayerState.WorldSpawned && e.Command is not LoadWorld) {
            log.Warning($"Command {e.Command.GetType().FullName} received, but the world isn't spawned yet");
            return;
        }
        log.Trace(() => $"Command {e.Command.GetType().Name} received from server");
        PatchControl.RunWithDisabledPatches(() => e.Command.Execute());
    }

    #endregion

    private void OnWorldSpawned() {
        switch (MultiplayerState.Role) {
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

        var go = UnityObject.CreateStaticWithComponent<
            DrawCursorComponent,
            WorldDebugSnapshotRunner
        >();
        go.DestroyOnLoad();

        MultiplayerState.WorldSpawned = true;
    }

}
