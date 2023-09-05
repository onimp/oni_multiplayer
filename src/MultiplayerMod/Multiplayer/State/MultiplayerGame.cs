using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Players;

namespace MultiplayerMod.Multiplayer.State;

[UsedImplicitly]
public class MultiplayerGame {

    public MultiplayerMode Mode { get; set; } = MultiplayerMode.None;
    public MultiplayerPlayers Players { get; private set; } = null!;
    public MultiplayerObjects Objects { get; private set; } = null!;

    private readonly DependencyContainer container;

    public MultiplayerGame(DependencyContainer container) {
        this.container = container;
        Refresh();
    }

    public void Refresh() {
        Mode = MultiplayerMode.None;
        Players = container.Resolve<MultiplayerPlayers>();
        Objects = container.Resolve<MultiplayerObjects>();
    }

}
