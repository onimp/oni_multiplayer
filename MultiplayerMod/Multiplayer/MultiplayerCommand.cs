using System;

namespace MultiplayerMod.Multiplayer;

[Serializable]
public abstract class MultiplayerCommand : IMultiplayerCommand {

    public Guid Id { get; } = Guid.NewGuid();

    public abstract void Execute();

    public override string ToString() => $"Command [{Id:N}] {GetType().Name}";

}
