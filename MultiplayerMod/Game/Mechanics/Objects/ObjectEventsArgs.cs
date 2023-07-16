using System;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.Mechanics.Objects;

[Serializable]
public record ObjectEventsArgs(
    GameObjectReference Target,
    Type MethodType,
    string MethodName,
    object[] Args
);
