using System;
using MultiplayerMod.Multiplayer.Chores.Synchronizers;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.Objects.Reference;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

// Host-authoritative work-completion timing (Layer 1). Chore creation + assignment are already replicated,
// so both machines run the same WorkChore on the same duplicant toward the same workable - but each runs
// the labour at its own rate (work speed depends on unsynced dupe attributes/skills/machinery), so left
// alone they finish at different times. The client is gated to never complete a synced work stint on its
// own (WorkChoreSynchronizer.WorkTickPostfix keeps the dupe animating at the workable); the host sends
// this the instant it finishes the stint, and the client's gated copy completes on its next work tick
// through the worker's own state machine - so completion stays in lockstep. One tiny message per
// completion, no per-tick progress streaming.
//
// The workable is resolved by the shared reference (its MultiplayerId, or its fixed grid cell for
// unregistered buildings / diggables). If it's already gone on the client (e.g. a just-dug cell removed by
// the terrain stream) the completion is moot and skipped.
[Serializable]
public class SyncWorkComplete : MultiplayerCommand {

    private readonly GameObjectReference reference;

    public SyncWorkComplete(GameObject workable) {
        reference = workable.GetReference();
    }

    public override void Execute(MultiplayerCommandContext context) {
        GameObject workable;
        try {
            workable = reference.Resolve();
        } catch (ObjectNotFoundException) {
            return;
        }
        WorkChoreSynchronizer.MarkHostCompleted(workable);
    }

}
