using JetBrains.Annotations;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Multiplayer.Players.Events;
using MultiplayerMod.Multiplayer.UI;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.CoreOperations;

[UsedImplicitly]
public class MultiplayerJoinRequestController {

    private readonly MultiplayerGame multiplayer;
    private readonly IMultiplayerClient client;

    public MultiplayerJoinRequestController(
        EventDispatcher events,
        MultiplayerGame multiplayer,
        IMultiplayerClient client
    ) {
        this.multiplayer = multiplayer;
        this.client = client;
        events.Subscribe<MultiplayerJoinRequestedEvent>(OnMultiplayerJoinRequested);
    }

    private void OnMultiplayerJoinRequested(MultiplayerJoinRequestedEvent @event) {
        multiplayer.Refresh(MultiplayerMode.Client);
        LoadOverlay.Show($"Connecting to {@event.Name}...");
        client.Connect(@event.Endpoint);
    }

}
