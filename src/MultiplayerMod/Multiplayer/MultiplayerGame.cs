using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Players;

namespace MultiplayerMod.Multiplayer;

[UsedImplicitly]
public class MultiplayerGame {

    public MultiplayerMode Mode { get; private set; }
    public MultiplayerPlayers Players { get; private set; } = null!;
    public MultiplayerObjects Objects { get; private set; } = null!;

    private readonly DependencyContainer container;
    private readonly ExecutionLevelManager levelManager;

    public MultiplayerGame(DependencyContainer container, ExecutionLevelManager levelManager) {
        this.container = container;
        this.levelManager = levelManager;
        Refresh(MultiplayerMode.Disabled);
    }

    public void Refresh(MultiplayerMode mode) {
        Mode = mode;
        Players = new MultiplayerPlayers();
        Objects = container.Resolve<MultiplayerObjects>();

        var executionLevel = mode == MultiplayerMode.Disabled ? ExecutionLevel.System : ExecutionLevel.Multiplayer;
        levelManager.BaseLevel = executionLevel;
    }

}
