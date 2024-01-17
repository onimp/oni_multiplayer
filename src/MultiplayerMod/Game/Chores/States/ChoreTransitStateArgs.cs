using System;
using System.Collections.Generic;

namespace MultiplayerMod.Game.Chores.States;

[Serializable]
public record ChoreTransitStateArgs(
    Chore Chore,
    string? TargetState,
    Dictionary<int, object?> Args
);
