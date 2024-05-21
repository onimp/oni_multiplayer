using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.Game.Chores.States;
using MultiplayerMod.Multiplayer.Chores.Commands;
using MultiplayerMod.Multiplayer.Chores.Commands.States;
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

    private EventSubscriptions subscriptions = null!;

    public HostEventsBinder(IMultiplayerServer server, WorldManager worldManager, EventDispatcher events) {
        this.server = server;
        this.worldManager = worldManager;
        this.events = events;

        Bind();
        BindChores();
    }

    private void BindChores() {
        events.Subscribe<ChoreCreatedEvent>(@event => server.Send(
            new CreateChore(@event.Id, @event.Type, @event.Arguments),
            MultiplayerCommandOptions.SkipHost
        ));
    }

    private void Bind() {
        log.Debug("Binding host events");

        events.Subscribe<GameStartedEvent>(OnGameStarted);
        events.Subscribe<StopMultiplayerEvent>(OnStopMultiplayer);

        ChoreEvents.CreateNewChore += args => server.Send(
            new CreateChore(args.ChoreId, args.ChoreType, args.Args),
            MultiplayerCommandOptions.SkipHost
        );
        ChoreStateEvents.OnStateEnter += args => server.Send(
            AllowStateTransition.EnterTransition(args),
            MultiplayerCommandOptions.SkipHost
        );
        ChoreStateEvents.OnEnterMoveTo += args => server.Send(
            new StartMove(args),
            MultiplayerCommandOptions.SkipHost
        );
        ChoreStateEvents.OnStateExit += args => server.Send(
            AllowStateTransition.ExitTransition(args),
            MultiplayerCommandOptions.SkipHost
        );
        ChoreStateEvents.OnExitMoveTo += args => server.Send(
            new TransitToState(args.Chore, args.TargetState),
            MultiplayerCommandOptions.SkipHost
        );
        ChoreStateEvents.OnStateTransition += args => server.Send(
            new TransitToState(args.Chore, args.TargetState),
            MultiplayerCommandOptions.SkipHost
        );
        ChoreStateEvents.OnStateUpdate += args => server.Send(
            new SetStateArguments(args),
            MultiplayerCommandOptions.SkipHost
        );
        ChoreStateEvents.OnUpdateMoveTo += args => server.Send(
            new UpdateMove(args),
            MultiplayerCommandOptions.SkipHost
        );
        ChoreStateEvents.OnStateEventHandler += args => server.Send(
            new SetStateArguments(args),
            MultiplayerCommandOptions.SkipHost
        );
    }

    private void OnGameStarted(GameStartedEvent @event) {
        if (@event.Multiplayer.Mode != MultiplayerMode.Host)
            return;

        subscriptions = [
            events.Subscribe<WorldSavedEvent>(_ => worldManager.Sync()),
            events.Subscribe<DebugSnapshotAvailableEvent>(e => server.Send(new SyncWorldDebugSnapshot(e.Snapshot)))
        ];
    }

    private void OnStopMultiplayer(StopMultiplayerEvent @event) {
        if (@event.Multiplayer.Mode != MultiplayerMode.Host)
            return;

        subscriptions.Cancel();
    }

}
