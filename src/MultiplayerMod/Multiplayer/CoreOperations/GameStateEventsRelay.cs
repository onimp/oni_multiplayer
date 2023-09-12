using JetBrains.Annotations;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Game;
using MultiplayerMod.Game.UI.Screens.Events;
using MultiplayerMod.Game.World;
using MultiplayerMod.Multiplayer.CoreOperations.Events;

namespace MultiplayerMod.Multiplayer.CoreOperations;

[UsedImplicitly]
public class GameStateEventsRelay {

    private readonly EventDispatcher events;
    private readonly MultiplayerGame multiplayer;

    public GameStateEventsRelay(EventDispatcher events, MultiplayerGame multiplayer) {
        this.events = events;
        this.multiplayer = multiplayer;

        GameEvents.GameStarted += OnGameStarted;
        PauseScreenEvents.GameQuit += OnGameQuit;
        SaveLoaderEvents.WorldSaved += OnWorldSave;
    }

    private void OnWorldSave() => events.Dispatch(new WorldSavedEvent());

    private void OnGameQuit() => events.Dispatch(new GameQuitEvent(multiplayer));

    private void OnGameStarted() => events.Dispatch(new GameStartedEvent(multiplayer));

}
