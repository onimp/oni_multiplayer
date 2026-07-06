using System;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

// Host-authoritative despawn of a runtime object addressed by its shared MultiplayerId. Generic on purpose:
// the critter lifecycle (egg consumed on hatch, baby consumed on grow-up) drives it, and it doubles as a
// death safety net. The counterpart to the shared-id SPAWN commands (SyncSpawnCritter / the id-carrying
// SyncBuildingComplete / SyncSpawnPickupable) - together they let the host add and remove objects on the
// client under an id both machines resolve, instead of leaving population drift to the daily hard-sync.
//
// Resolves by id and deletes. Graceful: a target that was never replicated to this client, or that the
// client already removed on its own (e.g. its local copy self-deleted), throws ObjectNotFoundException on
// Resolve - swallowed here so a stale despawn is a no-op rather than a command-handler dump.
// MultiplayerInstance.OnCleanUp unregisters the id automatically when the GameObject is destroyed.
[Serializable]
public class SyncDespawnEntity : MultiplayerCommand {

    private readonly MultiplayerId id;

    public SyncDespawnEntity(MultiplayerId id) {
        this.id = id;
    }

    public override void Execute(MultiplayerCommandContext context) {
        GameObject gameObject;
        try {
            gameObject = new MultiplayerIdReference(id).Resolve();
        } catch (ObjectNotFoundException) {
            return;
        }
        gameObject.DeleteObject();
    }

}
