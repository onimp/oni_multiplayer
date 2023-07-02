using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Objects;

public class MultiplayerIdentityProvider {

    public long NextObjectId { get; set; }

    private readonly IMultiplayerClient client;

    public MultiplayerIdentityProvider() {
        client = Container.Get<IMultiplayerClient>();
    }

    public MultiplayerId GetNextId() => new(client.Player, NextObjectId++);

}
