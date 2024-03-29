﻿using System;

namespace MultiplayerMod.Multiplayer.Commands;

[Serializable]
public abstract class MultiplayerCommand : IMultiplayerCommand {

    public Guid Id { get; } = Guid.NewGuid();

    public abstract void Execute(MultiplayerCommandContext context);

    public override string ToString() => $"Command [{Id:N}] {GetType().Name}";

}
