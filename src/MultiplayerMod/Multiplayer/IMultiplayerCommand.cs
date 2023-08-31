using System;
using MultiplayerMod.ModRuntime;

namespace MultiplayerMod.Multiplayer;

public interface IMultiplayerCommand {
    Guid Id { get; }
    void Execute(Runtime runtime);
}
