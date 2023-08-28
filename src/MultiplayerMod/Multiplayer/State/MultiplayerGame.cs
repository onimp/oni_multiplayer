using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.State;

[UsedImplicitly]
public class MultiplayerGame {

    public MultiplayerRole Role { get; set; } = MultiplayerRole.None;
    public MultiplayerState State { get; set; } = null!;
    public MultiplayerObjects Objects { get; private set; } = null!;

    private readonly DependencyContainer container;

    public MultiplayerGame(DependencyContainer container) {
        this.container = container;
        Refresh();
    }

    public void Refresh() {
        Role = MultiplayerRole.None;
        State = container.Resolve<MultiplayerState>();
        Objects = container.Resolve<MultiplayerObjects>();
    }

}
