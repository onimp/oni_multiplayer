using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.Chores.Commands;
using MultiplayerMod.Multiplayer.Chores.Events;
using MultiplayerMod.Multiplayer.Commands.Debug;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
using MultiplayerMod.Multiplayer.World;
using MultiplayerMod.Multiplayer.World.Debug;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.CoreOperations.Binders;

[Dependency, UsedImplicitly]
public class HostEventsBinder {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<HostEventsBinder>();
    private readonly IMultiplayerServer server;
    private readonly WorldManager worldManager;
    private readonly EventDispatcher events;
    private readonly MultiplayerGame multiplayer;

    private EventSubscriptions subscriptions = null!;

    public HostEventsBinder(
        IMultiplayerServer server,
        WorldManager worldManager,
        EventDispatcher events,
        MultiplayerGame multiplayer
    ) {
        this.server = server;
        this.worldManager = worldManager;
        this.events = events;
        this.multiplayer = multiplayer;

        Bind();
        BindChores();
    }

    private void BindChores() {
        events.Subscribe<ChoreCreatedEvent>(@event => server.Send(
            new CreateChore(@event.Id, @event.Type, @event.Arguments)
        ));

        // ChoreCleanupEvent fires on both host and client (every chore's Cleanup), so gate to host and to
        // chores that were actually replicated (Id != null). Tells clients to end their copy in lockstep.
        events.Subscribe<ChoreCleanupEvent>(@event => {
            if (multiplayer.Mode != MultiplayerMode.Host || @event.Id == null || server.Clients.Count == 0)
                return;
            server.Send(new CompleteChore(@event.Id));
        });
    }

    private void Bind() {
        log.Debug("Binding host events");

        events.Subscribe<GameStartedEvent>(OnGameStarted);
        events.Subscribe<StopMultiplayerEvent>(OnStopMultiplayer);
    }

    private void OnGameStarted(GameStartedEvent @event) {
        if (@event.Multiplayer.Mode != MultiplayerMode.Host)
            return;

        subscriptions = [
            events.Subscribe<WorldSavedEvent>(_ => worldManager.Sync()),
            events.Subscribe<DebugSnapshotAvailableEvent>(e => server.SendAll(new SyncWorldDebugSnapshot(e.Snapshot)))
        ];
    }

    private void OnStopMultiplayer(StopMultiplayerEvent @event) {
        if (@event.Multiplayer.Mode != MultiplayerMode.Host)
            return;

        subscriptions.Cancel();
    }

}
