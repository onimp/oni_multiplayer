using System;
using MultiplayerMod.ModRuntime;

namespace MultiplayerMod.Multiplayer;

[Serializable]
public abstract class MultiplayerCommand : IMultiplayerCommand {

    public Guid Id { get; } = Guid.NewGuid();

    public abstract void Execute(Runtime runtime);

    public override string ToString() => $"Command [{Id:N}] {GetType().Name}";

}
