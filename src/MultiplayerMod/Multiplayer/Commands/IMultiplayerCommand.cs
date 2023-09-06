using System;

namespace MultiplayerMod.Multiplayer.Commands;

public interface IMultiplayerCommand {
    Guid Id { get; }
    void Execute(MultiplayerCommandContext context);
}
