using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Players;

namespace MultiplayerMod.Multiplayer;

[UsedImplicitly]
public class MultiplayerGame {

    public MultiplayerMode Mode { get; private set; }
    public MultiplayerPlayers Players { get; private set; } = null!;
    public MultiplayerObjects Objects { get; private set; } = null!;

    private readonly DependencyContainer container;
    private readonly EventDispatcher eventDispatcher;

    public MultiplayerGame(DependencyContainer container, EventDispatcher eventDispatcher) {
        this.container = container;
        this.eventDispatcher = eventDispatcher;
        Refresh(MultiplayerMode.Disabled);
    }

    public void Refresh(MultiplayerMode mode) {
        Mode = mode;
        Players = new MultiplayerPlayers();
        Objects = container.Resolve<MultiplayerObjects>();

        eventDispatcher.Dispatch(new MultiplayerModeChangedEvent(mode));
    }

}
