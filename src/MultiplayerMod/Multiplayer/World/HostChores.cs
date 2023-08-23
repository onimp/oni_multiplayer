using System;
using System.Collections.Generic;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.World;

public static class HostChores {

    public static Dictionary<ComponentReference<ChoreConsumer>, Queue<Func<Chore.Precondition.Context?>>> Index { get; } =
        new();

}
