using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
using MultiplayerMod.Multiplayer.Players.Events;
using MultiplayerMod.Multiplayer.UI.Overlays;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.CoreOperations;

[Dependency, UsedImplicitly]
public class MultiplayerJoinRequestController {

    private readonly EventDispatcher events;
    private readonly IMultiplayerClient client;

    public MultiplayerJoinRequestController(EventDispatcher events, IMultiplayerClient client) {
        this.events = events;
        this.client = client;

        events.Subscribe<MultiplayerJoinRequestedEvent>(OnMultiplayerJoinRequested);
    }

    private void OnMultiplayerJoinRequested(MultiplayerJoinRequestedEvent @event) {
        events.Dispatch(new MultiplayerModeSelectedEvent(MultiplayerMode.Client));
        MultiplayerStatusOverlay.Show($"Connecting to {@event.HostName}...");
        client.Connect(@event.Endpoint);
    }

}
