using System;

namespace MultiplayerMod.Game.Chores;

public record CreateNewChoreArgs(
    Type ChoreType,
    object?[] Args
);
