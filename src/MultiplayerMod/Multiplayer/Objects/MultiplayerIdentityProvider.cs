using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Objects;

[Dependency, UsedImplicitly]
public class MultiplayerIdentityProvider {

    public long NextObjectId { get; set; }

    private readonly IMultiplayerClient client;

    public MultiplayerIdentityProvider(IMultiplayerClient client) {
        this.client = client;
    }

    public MultiplayerId GetNextId() => new(client.Id, NextObjectId++);

}
