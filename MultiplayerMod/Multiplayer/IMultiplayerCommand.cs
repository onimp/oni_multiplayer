using System;

namespace MultiplayerMod.Multiplayer;

public interface IMultiplayerCommand {
    Guid Id { get; }
    void Execute();
}
