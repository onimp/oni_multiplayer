using System.Reflection;

namespace MultiplayerMod.Game.Mechanics.Objects;

public record ComponentEventsArgs(
    KMonoBehaviour Component,
    MethodBase Method,
    object[] Args
);
