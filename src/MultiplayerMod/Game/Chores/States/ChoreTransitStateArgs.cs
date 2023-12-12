using System;
using System.Collections.Generic;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Game.Chores.States;

[Serializable]
public record ChoreTransitStateArgs(
    MultiplayerId ChoreId,
    string TargetState,
    Dictionary<int, object> Args
);
