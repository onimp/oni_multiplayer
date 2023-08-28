using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Objects;

// ReSharper disable once ClassNeverInstantiated.Global
public class MultiplayerIdentityProvider {

    public long NextObjectId { get; set; }

    private readonly IMultiplayerClient client;

    public MultiplayerIdentityProvider(IMultiplayerClient client) {
        this.client = client;
    }

    public MultiplayerId GetNextId() => new(client.Player, NextObjectId++);

}
