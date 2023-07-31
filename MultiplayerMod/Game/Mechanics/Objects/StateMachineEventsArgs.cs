using System;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.Mechanics.Objects;

public record StateMachineEventsArgs(
    StateMachineReference Target,
    Type MethodType,
    string MethodName,
    object[] Args
);
