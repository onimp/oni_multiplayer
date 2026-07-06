using System;
using MultiplayerMod.Multiplayer.Objects.Reference;
using MultiplayerMod.Multiplayer.World;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

// Host-authoritative building "active" state (Operational.IsActive). A machine's working animation, its
// operating status item and uptime are all driven by IsActive, which the game recomputes from local
// conditions (power, connections, and - crucially - input material in the building's own storage). The
// client's materials/sim economy isn't fully replicated, so a carbon scrubber that is busy on the host sits
// idle on the client (no input in reach) and never plays its working animation even though the host's
// outputs are streamed in. Rather than replicate every machine's inputs, the host streams the authoritative
// IsActive for each building and the client stomps it (OperationalActiveSynchronizer suppresses the client's
// own SetActive so its local logic can't fight the synced value and flicker the animation). Buildings are
// resolved by MultiplayerId / grid reference and skipped gracefully if not yet replicated on this client.
[Serializable]
public class SyncBuildingActive : MultiplayerCommand {

    private readonly Entry[] entries;

    public SyncBuildingActive(Entry[] entries) {
        this.entries = entries;
    }

    public SyncBuildingActive(ComponentReference<Operational> reference, bool active)
        : this([new Entry { Reference = reference, Active = active }]) { }

    public override void Execute(MultiplayerCommandContext context) {
        foreach (var entry in entries) {
            Operational operational;
            try {
                operational = entry.Reference.Resolve();
            } catch (ObjectNotFoundException) {
                continue;
            }
            OperationalActiveSynchronizer.ApplyHostValue(operational, entry.Active);
        }
    }

    [Serializable]
    public class Entry {
        public ComponentReference<Operational> Reference = null!;
        public bool Active;
    }

}
