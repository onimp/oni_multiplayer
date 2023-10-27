using System.Collections.Generic;
using System.Linq;

namespace MultiplayerMod.Multiplayer.Chores;

public class MultiplayerHostChores {

    // WorldId -> Provider -> PendingChores
    public static Dictionary<int, Dictionary<ChoreProvider, List<MultiplayerHostChore>>> HostDriversPool = new();

    public static void RemovePooledChore(MultiplayerHostChore chore) {
        HostDriversPool[chore.WorldId][chore.Provider].Remove(chore);
    }

    public static MultiplayerHostChore? FindPooledChore(ref Chore.Precondition.Context context) {
        var pool = HostDriversPool;
        var chore = context.chore;
        if (!pool.TryGetValue(chore.gameObject.GetMyParentWorldId(), out var pooledWorldChores))
            return null;
        if (!pooledWorldChores.TryGetValue(chore.provider, out var pooledProviderChores))
            return null;

        return pooledProviderChores
            .Where(it => it.ChoreType == chore.choreType)
            .Where(it => it.Target == chore.target.gameObject)
            .FirstOrDefault(it => it.ChoreInstanceType == chore.GetType());
    }

}
