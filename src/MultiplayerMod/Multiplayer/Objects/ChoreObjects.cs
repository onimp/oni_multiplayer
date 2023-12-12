using System;
using System.Collections.Generic;

namespace MultiplayerMod.Multiplayer.Objects;

public static class ChoreObjects {
    private static readonly Dictionary<MultiplayerId, Chore> choreMultiplayerIds = new();

    public static Chore? GetChore(MultiplayerId multiplayerId) {
        choreMultiplayerIds.TryGetValue(multiplayerId, out var result);
        return result;
    }

    public static MultiplayerId Register(this Chore chore, MultiplayerId? multiplayerId = null) {
        var id = multiplayerId ?? new MultiplayerId(Guid.NewGuid());
        choreMultiplayerIds[id] = chore;
        return id;
    }
}
