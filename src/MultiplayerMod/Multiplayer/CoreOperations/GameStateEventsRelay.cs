using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.Game;
using MultiplayerMod.Game.UI.Screens.Events;
using MultiplayerMod.Game.World;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.CoreOperations;

[Dependency, UsedImplicitly]
public class GameStateEventsRelay {

    private readonly EventDispatcher events;
    private readonly MultiplayerGame multiplayer;
    private readonly UnityTaskScheduler scheduler;

    public GameStateEventsRelay(EventDispatcher events, MultiplayerGame multiplayer, UnityTaskScheduler scheduler) {
        this.events = events;
        this.multiplayer = multiplayer;
        this.scheduler = scheduler;

        GameEvents.GameStarted += OnGameStarted;
        PauseScreenEvents.GameQuit += OnGameQuit;
        SaveLoaderEvents.WorldSaved += OnWorldSave;
        SaveLoaderEvents.WorldLoading += OnWorldLoading;
    }

    private void OnWorldLoading() => events.Dispatch(new WorldLoadingEvent());

    private void OnWorldSave() => events.Dispatch(new WorldSavedEvent());

    private void OnGameQuit() => events.Dispatch(new GameQuitEvent());

    private void OnGameStarted() {
        scheduler.Run(() => {
            events.Dispatch(new GameReadyEvent());
            events.Dispatch(new WorldStateInitializingEvent());
            events.Dispatch(new GameStartedEvent(multiplayer));
        });
    }

}
