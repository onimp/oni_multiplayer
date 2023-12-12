using System;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Game.Chores;

public record CreateNewChoreArgs(
    MultiplayerId ChoreId,
    Type ChoreType,
    object?[] Args
);
