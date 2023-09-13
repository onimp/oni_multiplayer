using System;

namespace MultiplayerMod.Multiplayer.Players;

[Serializable]
public record PlayerIdentity {
    public Guid Value { get; } = Guid.NewGuid();

    public override string ToString() => Value.ToString();
}
