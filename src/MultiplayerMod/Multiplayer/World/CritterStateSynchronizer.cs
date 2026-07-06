using System.Collections.Generic;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Gameplay;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Network;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.World;

// Host-authoritative replication of critter VISIBLE state (position / facing / health). Critters run their
// own AI on the client (their chores/monitors are not replicated), so they wander to different cells and
// take different damage until the daily hard-sync corrects it - the most visible mid-game critter
// divergence. This streams the host's authoritative state to clients continuously instead (see
// SyncCritterState for the model and the applied-state details).
//
// Enumerates Components.Brains and keeps only CreatureBrain (dupes/robots are handled by
// DuplicantStateSynchronizer and their own chore replication). Like DuplicantStateSynchronizer we send
// every live critter in a single command each cadence tick on the reliable Gameplay lane; a client that
// hasn't replicated a given critter yet simply skips it (graceful resolve). Gated Host-only + level-active
// + at least one client, on a ~1 s cadence.
public class CritterStateSynchronizer : MultiplayerKMonoBehaviour, IRenderEveryTick {

    private const float sendPeriod = 1.0f;

    [InjectDependency]
    private readonly IMultiplayerServer server = null!;

    [InjectDependency]
    private readonly MultiplayerGame multiplayer = null!;

    [InjectDependency]
    private readonly ExecutionLevelManager manager = null!;

    private float lastTime;

    public void RenderEveryTick(float dt) {
        if (!manager.LevelIsActive(ExecutionLevel.Multiplayer) || multiplayer.Mode != MultiplayerMode.Host)
            return;
        if (server.Clients.Count == 0)
            return;
        if (GameClock.Instance.GetTime() - lastTime < sendPeriod)
            return;

        lastTime = GameClock.Instance.GetTime();

        var entries = BuildEntries();
        if (entries.Length > 0)
            server.Send(new SyncCritterState(entries));
    }

    private static SyncCritterState.CritterEntry[] BuildEntries() {
        var brains = global::Components.Brains.Items;
        var list = new List<SyncCritterState.CritterEntry>(brains.Count);
        foreach (var brain in brains) {
            if (brain is not CreatureBrain creatureBrain)
                continue;

            var gameObject = creatureBrain.gameObject;
            var entry = new SyncCritterState.CritterEntry {
                Reference = creatureBrain.GetReference<CreatureBrain>(),
                Position = gameObject.transform.GetPosition()
            };

            var facing = gameObject.GetComponent<Facing>();
            if (facing != null) {
                entry.HasFacing = true;
                entry.FacingLeft = facing.facingLeft;
            }

            var health = gameObject.GetComponent<Health>();
            if (health != null) {
                entry.HasHealth = true;
                entry.HitPoints = health.hitPoints;
                entry.CanBeIncapacitated = health.canBeIncapacitated;
            }

            // Current anim of the critter's KAnim controller so the client can match the visible action
            // (see SyncCritterState.ApplyAnim). Hash is of the anim name - stable across machines.
            var kbac = gameObject.GetComponent<KBatchedAnimController>();
            if (kbac != null) {
                var currentAnim = kbac.currentAnim;
                if (currentAnim.IsValid) {
                    entry.HasAnim = true;
                    entry.AnimHash = currentAnim.HashValue;
                    entry.AnimMode = (int) kbac.PlayMode;
                }
            }

            list.Add(entry);
        }
        return list.ToArray();
    }

}
