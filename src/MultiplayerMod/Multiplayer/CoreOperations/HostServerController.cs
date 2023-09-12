using JetBrains.Annotations;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
using MultiplayerMod.Multiplayer.UI;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.CoreOperations;

[UsedImplicitly]
public class HostServerController {

    private readonly IMultiplayerServer server;
    private readonly IMultiplayerClient client;

    public HostServerController(IMultiplayerServer server, IMultiplayerClient client, EventDispatcher events) {
        this.server = server;
        this.client = client;

        events.Subscribe<GameStartedEvent>(OnGameStarted);
        events.Subscribe<GameQuitEvent>(OnGameQuit);

        server.StateChanged += OnServerStateChanged;
    }

    private void OnServerStateChanged(MultiplayerServerState state) {
        if (state == MultiplayerServerState.Started)
            client.Connect(server.Endpoint);
    }

    private void OnGameStarted(GameStartedEvent @event) {
        if (!@event.IsHostMode)
            return;

        LoadOverlay.Show("Starting host...");
        server.Start();
    }

    private void OnGameQuit(GameQuitEvent @event) {
        if (!@event.IsHostMode)
            return;

        server.Stop();
    }

}
