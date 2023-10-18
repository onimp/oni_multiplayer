using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
using MultiplayerMod.Multiplayer.Players.Events;
using MultiplayerMod.Multiplayer.UI.Overlays;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.CoreOperations;

[Dependency, UsedImplicitly]
public class MultiplayerServerController {

    private readonly IMultiplayerServer server;
    private readonly IMultiplayerClient client;
    private readonly EventDispatcher events;

    public MultiplayerServerController(IMultiplayerServer server, IMultiplayerClient client, EventDispatcher events) {
        this.server = server;
        this.client = client;
        this.events = events;

        events.Subscribe<GameStartedEvent>(OnGameStarted);
        events.Subscribe<StopMultiplayerEvent>(OnStopMultiplayer);

        server.StateChanged += OnServerStateChanged;
    }

    private void OnServerStateChanged(MultiplayerServerState state) {
        if (state == MultiplayerServerState.Started)
            client.Connect(server.Endpoint);
    }

    private void OnGameStarted(GameStartedEvent @event) {
        if (@event.Multiplayer.Mode != MultiplayerMode.Host)
            return;

        MultiplayerStatusOverlay.Show("Starting host...");
        events.Subscribe<PlayersReadyEvent>(
            (_, subscription) => {
                MultiplayerStatusOverlay.Close();
                subscription.Cancel();
            }
        );

        server.Start();
    }

    private void OnStopMultiplayer(StopMultiplayerEvent @event) {
        if (@event.Multiplayer.Mode != MultiplayerMode.Host)
            return;

        server.Stop();
    }

}
