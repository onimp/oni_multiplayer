using System;
using MultiplayerMod.Multiplayer.Objects.Reference;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

// Host-authoritative replication of a critter's VISIBLE state - position / facing / health. Unlike
// duplicants, critter behaviour (chores/monitors) is NOT replicated: the client keeps running each
// critter's own AI, so mid-cycle a critter wanders to a different cell, takes different damage, and
// drifts from the host until the daily hard-sync reloads the world. This streams the host's authoritative
// position + facing + health and the client STOMPS it, the same self-healing re-stream model as
// SyncDuplicantState / SyncSimCells (we don't suppress the client's local sim; a full re-stream each
// cadence tick corrects any between-tick drift).
//
// Each critter is resolved by its CreatureBrain ComponentReference (shared MultiplayerId - critters present
// since the last hard-sync are registered by SaveGameObjectsInitializer's KPrefabID sweep). A critter that
// isn't replicated on this client yet - e.g. one hatched/born mid-cycle with an independent instance id -
// is skipped per-entry (that population reconciles at the next hard-sync).
//
// Rides the default reliable Gameplay lane; the payload is small (a Vector3 + a few flags per critter).
[Serializable]
public class SyncCritterState : MultiplayerCommand {

    private readonly CritterEntry[] critters;

    public SyncCritterState(CritterEntry[] critters) {
        this.critters = critters;
    }

    public override void Execute(MultiplayerCommandContext context) {
        foreach (var entry in critters) {
            GameObject gameObject;
            try {
                // Resolve gracefully: a critter may not be replicated to this client yet or may have been
                // destroyed. Catch per-entry (rather than letting ObjectNotFoundException bubble to the
                // command handler) so one missing critter never aborts the rest of the batch.
                gameObject = entry.Reference.Resolve().gameObject;
            } catch (ObjectNotFoundException) {
                continue;
            }

            gameObject.transform.SetPosition(entry.Position);

            if (entry.HasFacing) {
                var facing = gameObject.GetComponent<Facing>();
                if (facing != null)
                    facing.SetFacing(entry.FacingLeft);
            }

            if (entry.HasHealth)
                ApplyHealth(gameObject, entry);

            if (entry.HasAnim)
                ApplyAnim(gameObject, entry);
        }
    }

    // The client keeps running each critter's own AI, so its KAnim controller plays a locally-chosen anim
    // (idle vs eat vs sleep vs poop ...) that drifts from the host. We stomp the host's current anim so the
    // visible action matches the host-streamed position - but ONLY when it actually differs, so we don't
    // restart (and stutter) a still-playing loop every cadence tick. The client AI may re-drive the anim on
    // its next state transition; long-lived states (idle / sleep / walk loops) still hold host-side between
    // transitions, which is the drift a player actually notices. The anim hash is a HashedString of the anim
    // NAME - stable across machines for the same species prefab - so no per-species mapping table is needed.
    private static void ApplyAnim(GameObject gameObject, CritterEntry host) {
        if (host.AnimHash == 0)
            return;
        var kbac = gameObject.GetComponent<KBatchedAnimController>();
        if (kbac == null || kbac.currentAnim.HashValue == host.AnimHash)
            return;
        kbac.Play(new HashedString(host.AnimHash), (KAnim.PlayMode) host.AnimMode);
    }

    private static void ApplyHealth(GameObject gameObject, CritterEntry host) {
        var health = gameObject.GetComponent<Health>();
        if (health == null)
            return;

        health.canBeIncapacitated = host.CanBeIncapacitated;
        if (Mathf.Approximately(health.hitPoints, host.HitPoints))
            return;

        // Raw-set the value, then run the game's own change handler so State / health bar recompute from it
        // (and death fires if the host drove it to 0). Only when it actually changed, so we don't re-fire
        // the health-changed event every cadence tick.
        health.hitPoints = host.HitPoints;
        health.OnHealthChanged(0f);
    }

    // ---- Wire payload -------------------------------------------------------------------------------

    [Serializable]
    public class CritterEntry {
        public ComponentReference<CreatureBrain> Reference = null!;
        public Vector3 Position;
        public bool HasFacing;
        public bool FacingLeft;
        public bool HasHealth;
        public float HitPoints;
        public bool CanBeIncapacitated;
        public bool HasAnim;
        public int AnimHash;
        public int AnimMode;
    }

}
