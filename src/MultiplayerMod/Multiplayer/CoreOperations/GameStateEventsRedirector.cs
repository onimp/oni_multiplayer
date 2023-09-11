using JetBrains.Annotations;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Game;
using MultiplayerMod.Game.UI.Screens.Events;
using MultiplayerMod.Game.World;
using MultiplayerMod.Multiplayer.CoreOperations.Events;

namespace MultiplayerMod.Multiplayer.CoreOperations;

[UsedImplicitly]
public class GameStateEventsRedirector {

    private readonly EventDispatcher events;
    private readonly MultiplayerGame multiplayer;

    public GameStateEventsRedirector(EventDispatcher events, MultiplayerGame multiplayer) {
        this.events = events;
        this.multiplayer = multiplayer;

        GameEvents.GameStarted += OnGameStarted;
        PauseScreenEvents.GameQuit += OnGameQuit;
        SaveLoaderEvents.WorldSaved += OnWorldSave;
    }

    private void OnWorldSave() => events.Dispatch(new WorldSavedEvent());

    private void OnGameQuit() {
        events.Dispatch(new GameQuitEvent());
        events.Dispatch(new MultiplayerGameQuittingEvent(multiplayer));
    }

    private void OnGameStarted() {
        events.Dispatch(new GameStartedEvent());
        events.Dispatch(new MultiplayerGameStartingEvent(multiplayer));
    }

}
