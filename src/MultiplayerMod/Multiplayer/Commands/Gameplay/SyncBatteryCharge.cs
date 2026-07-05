using System;
using MultiplayerMod.Multiplayer.Objects.Reference;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

// Host-authoritative battery charge. Power generation / consumption is simulated independently on each
// machine, and on the client generators are starved (fuel delivery rides the unsynced materials economy),
// so stored charge drifts - a battery full on the host can read empty on the client, flipping whole
// circuits (and everything they power) on and off. Rather than replicate the whole power sim, the host
// streams each battery's stored joules and the client stomps it - the same self-healing re-stream model as
// SyncDuplicantState / SyncSimCells, so a between-tick divergence corrects on the next tick. Batteries are
// resolved by MultiplayerId / grid reference and skipped gracefully if not yet replicated on this client.
// Generators' transient output buffers are left to the daily hard-sync.
[Serializable]
public class SyncBatteryCharge : MultiplayerCommand {

    private readonly BatteryEntry[] batteries;

    public SyncBatteryCharge(BatteryEntry[] batteries) {
        this.batteries = batteries;
    }

    public override void Execute(MultiplayerCommandContext context) {
        foreach (var entry in batteries) {
            Battery battery;
            try {
                battery = entry.Reference.Resolve();
            } catch (ObjectNotFoundException) {
                continue;
            }
            battery.joulesAvailable = Mathf.Clamp(entry.Joules, 0f, battery.capacity);
        }
    }

    [Serializable]
    public class BatteryEntry {
        public ComponentReference<Battery> Reference = null!;
        public float Joules;
    }

}
