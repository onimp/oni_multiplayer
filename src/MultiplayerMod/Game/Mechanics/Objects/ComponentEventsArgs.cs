using System;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.Mechanics.Objects;

public record ComponentEventsArgs(
    ComponentReference Target,
    Type MethodType,
    string MethodName,
    object[] Args
);
