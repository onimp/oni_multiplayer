using JetBrains.Annotations;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.Multiplayer.Commands.Chores;
using MultiplayerMod.Multiplayer.Commands.Debug;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
using MultiplayerMod.Multiplayer.World;
using MultiplayerMod.Multiplayer.World.Debug;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.CoreOperations.Binders;

[UsedImplicitly]
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
    }

    public void Bind() {
        log.Debug("Binding host events");

        eventDispatcher.Subscribe<GameStartedEvent>(OnGameStarted);
        eventDispatcher.Subscribe<GameQuitEvent>(OnGameQuit);

        ChoreConsumerEvents.FindNextChore += p => server.Send(new FindNextChore(p), MultiplayerCommandOptions.SkipHost);
    }

    [RequireMultiplayerMode(MultiplayerMode.Host)]
    private void OnGameStarted(GameStartedEvent _) {
        subscriptions = new EventSubscriptions {
            eventDispatcher.Subscribe<WorldSavedEvent>(_ => worldManager.Sync()),
            eventDispatcher.Subscribe<DebugSnapshotAvailableEvent>(
                e => server.Send(new SyncWorldDebugSnapshot(e.Snapshot))
            )
        };
    }

    [RequireMultiplayerMode(MultiplayerMode.Host)]
    private void OnGameQuit(GameQuitEvent _) => subscriptions.Cancel();

}
