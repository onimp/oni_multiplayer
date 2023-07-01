using MultiplayerMod.Game.World;

namespace MultiplayerMod.Multiplayer.Objects;

public static class MultiplayerIdentityProvider {

    private static long nextId;

    public static long GetNextId() => nextId++;

    static MultiplayerIdentityProvider() {
        WorldGenSpawnerEvents.Spawned += () => { nextId = KPrefabID.NextUniqueID; };
    }

}
