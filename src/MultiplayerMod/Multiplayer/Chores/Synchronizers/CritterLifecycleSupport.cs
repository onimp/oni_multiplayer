using MultiplayerMod.Multiplayer.Objects;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Chores.Synchronizers;

// Shared host-side helpers for the critter lifecycle synchronizers (CritterLayEgg / CritterHatch /
// CritterGrowUp). See behavior-chore/critter sync docs and SyncSpawnCritter for the model.
public static class CritterLifecycleSupport {

    // Locate the critter a lifecycle method just spawned, so the host can register + replicate it. The game
    // spawns babies/adults into a LOCAL variable a Harmony postfix can't read, so we identify it structurally
    // instead: it is the only CreatureBrain of the expected prefab tag, at the spawn cell, that has NOT yet
    // been registered with a shared MultiplayerId (Id == null). Everything present since the last hard-sync,
    // and every critter born earlier this session, already carries an id - so the freshly spawned one is
    // unambiguous. SetActive(true) runs inside the spawning method, so the newborn is already in
    // Components.Brains by the time the postfix runs.
    public static GameObject? FindUnregisteredCritter(Tag prefabTag, int cell) {
        foreach (var brain in global::Components.Brains.Items) {
            if (brain is not CreatureBrain)
                continue;

            var gameObject = brain.gameObject;
            if (gameObject.GetComponent<KPrefabID>().PrefabTag != prefabTag)
                continue;
            if (Grid.PosToCell(gameObject.transform.GetPosition()) != cell)
                continue;

            var instance = gameObject.GetComponent<MultiplayerInstance>();
            if (instance == null || instance.Id != null)
                continue;

            return gameObject;
        }
        return null;
    }

}
