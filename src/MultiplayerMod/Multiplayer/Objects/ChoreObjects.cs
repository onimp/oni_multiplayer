using System;
using System.Collections.Generic;

namespace MultiplayerMod.Multiplayer.Objects;

public static class ChoreObjects {
    private static readonly Dictionary<MultiplayerId, Chore> idChoreDictionary = new();
    private static readonly Dictionary<Chore, MultiplayerId> choreIdDictionary = new();

    public static void Clear() {
        idChoreDictionary.Clear();
        choreIdDictionary.Clear();
    }
    public static Chore GetChore(MultiplayerId multiplayerId) => idChoreDictionary[multiplayerId];

    public static MultiplayerId Register(this Chore chore, MultiplayerId? multiplayerId = null) {
        var id = multiplayerId ?? new MultiplayerId(Guid.NewGuid());
        idChoreDictionary[id] = chore;
        choreIdDictionary[chore] = id;
        return id;
    }

    public static MultiplayerId MultiplayerId(this Chore chore) => choreIdDictionary[chore];
}
