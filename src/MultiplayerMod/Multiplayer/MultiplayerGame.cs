using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Players;

namespace MultiplayerMod.Multiplayer;

[Dependency, UsedImplicitly]
public class MultiplayerGame {

    public MultiplayerMode Mode { get; private set; }
    public MultiplayerPlayers Players { get; private set; } = null!;
    public MultiplayerObjects Objects { get; private set; } = null!;

    private readonly IDependencyContainer container;

    public MultiplayerGame(IDependencyContainer container) {
        this.container = container;
        Refresh(MultiplayerMode.Client);
    }

    public void Refresh(MultiplayerMode mode) {
        Mode = mode;
        Players = new MultiplayerPlayers();
        Objects = new MultiplayerObjects(container.Get<MultiplayerIdentityProvider>());
    }

}
