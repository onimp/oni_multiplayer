using System.Collections.Generic;

namespace MultiplayerMod.Multiplayer.World;

public static class HostChores {

    public static Dictionary<int, Queue<Chore.Precondition.Context>> Index { get; } = new();

}
