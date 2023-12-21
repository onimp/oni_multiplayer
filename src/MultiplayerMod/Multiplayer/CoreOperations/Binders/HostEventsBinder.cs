using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.Game.Chores.States;
using MultiplayerMod.Multiplayer.Commands.Chores;
using MultiplayerMod.Multiplayer.Commands.Chores.States;
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
    private readonly EventDispatcher eventDispatcher;

    private EventSubscriptions subscriptions = null!;

    public HostEventsBinder(IMultiplayerServer server, WorldManager worldManager, EventDispatcher eventDispatcher) {
        this.server = server;
        this.worldManager = worldManager;
        this.eventDispatcher = eventDispatcher;

        Bind();
    }

    private void Bind() {
        log.Debug("Binding host events");

        eventDispatcher.Subscribe<GameStartedEvent>(OnGameStarted);
        eventDispatcher.Subscribe<StopMultiplayerEvent>(OnStopMultiplayer);

        ChoreConsumerEvents.FindNextChore += args => server.Send(
            new SetHostChore(args),
            MultiplayerCommandOptions.SkipHost
        );
        ChoreEvents.CreateNewChore += args => server.Send(
            new CreateHostChore(args),
            MultiplayerCommandOptions.SkipHost
        );
        ChoreStateEvents.OnStateTransition += args => server.Send(
            new TransitChoreToState(args),
            MultiplayerCommandOptions.SkipHost
        );
    }

    private void OnGameStarted(GameStartedEvent @event) {
        if (@event.Multiplayer.Mode != MultiplayerMode.Host)
            return;

        subscriptions = new EventSubscriptions {
            eventDispatcher.Subscribe<WorldSavedEvent>(_ => worldManager.Sync()),
            eventDispatcher.Subscribe<DebugSnapshotAvailableEvent>(
                e => server.Send(new SyncWorldDebugSnapshot(e.Snapshot))
            )
        };
    }

    private void OnStopMultiplayer(StopMultiplayerEvent @event) {
        if (@event.Multiplayer.Mode != MultiplayerMode.Host)
            return;

        subscriptions.Cancel();
    }

}
