using JetBrains.Annotations;
using MultiplayerMod.Core.Events;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Multiplayer.Players.Events;

namespace MultiplayerMod.Multiplayer.CoreOperations;

[UsedImplicitly]
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

        eventDispatcher.Subscribe<MultiplayerModeChangedEvent>(OnMultiplayerModeChanged);
        eventDispatcher.Subscribe<PlayerStateChangedEvent>(OnPlayerStateChangedEvent);
        eventDispatcher.Subscribe<MultiplayerGameQuittingEvent>(OnMultiplayerGameQuitting);
    }

    private void OnMultiplayerModeChanged(MultiplayerModeChangedEvent @event) {
        var executionLevel = @event.Mode == MultiplayerMode.Disabled
            ? ExecutionLevel.System
            : ExecutionLevel.Multiplayer;
        executionLevelManager.BaseLevel = executionLevel;
    }

    private void OnPlayerStateChangedEvent(PlayerStateChangedEvent @event) {
        if (multiplayer.Players.Current == @event.Player && @event.Player.State == PlayerState.Ready)
            executionLevelManager.BaseLevel = ExecutionLevel.Game;
    }

    private void OnMultiplayerGameQuitting(MultiplayerGameQuittingEvent _) {
        executionLevelManager.BaseLevel = ExecutionLevel.System;
    }

}
