using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Multiplayer.Players.Events;

namespace MultiplayerMod.Multiplayer.CoreOperations;

[Dependency, UsedImplicitly]
public class ExecutionLevelController {

    private readonly MultiplayerGame multiplayer;
    private readonly ExecutionLevelManager executionLevelManager;

    public ExecutionLevelController(
        EventDispatcher eventDispatcher,
        ExecutionLevelManager executionLevelManager,
        MultiplayerGame multiplayer
    ) {
        this.executionLevelManager = executionLevelManager;
        this.multiplayer = multiplayer;

        eventDispatcher.Subscribe<SinglePlayerModeSelectedEvent>(OnSinglePlayerModeSelected);
        eventDispatcher.Subscribe<MultiplayerModeSelectedEvent>(OnMultiplayerModeSelected);
        eventDispatcher.Subscribe<PlayerStateChangedEvent>(OnPlayerStateChangedEvent);
        eventDispatcher.Subscribe<GameQuitEvent>(OnGameQuit);
    }

    private void OnSinglePlayerModeSelected(SinglePlayerModeSelectedEvent @event) {
        executionLevelManager.BaseLevel = ExecutionLevel.System;
    }

    private void OnMultiplayerModeSelected(MultiplayerModeSelectedEvent @event) {
        executionLevelManager.BaseLevel = ExecutionLevel.Multiplayer;
    }

    private void OnPlayerStateChangedEvent(PlayerStateChangedEvent @event) {
        if (multiplayer.Players.Current == @event.Player && @event.Player.State == PlayerState.Ready)
            executionLevelManager.BaseLevel = ExecutionLevel.Game;
    }

    private void OnGameQuit(GameQuitEvent _) {
        executionLevelManager.BaseLevel = ExecutionLevel.System;
    }

}
