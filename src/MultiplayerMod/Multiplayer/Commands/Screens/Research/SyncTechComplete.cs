using System;
using MultiplayerMod.Core.Logging;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Research;

// Host-authoritative research unlock (see ResearchCompletionSynchronizer). The research-point sim runs
// independently on each machine and is otherwise only reconciled at the daily hard-sync - and on the
// client it barely accrues at all, since its research work is host-gated (WorkChoreSynchronizer). So a
// tech the host has just bought could stay locked on the client for a whole cycle, its new buildings/items
// unavailable. The unlock itself is a single boolean event though, so we replicate it directly: mark the
// client's TechInstance complete (which is what makes the tech's buildings/items available) and fire
// ResearchComplete so notifications / subscribers update, matching the host. Idempotent - a tech already
// complete on the client is left alone.
[Serializable]
public class SyncTechComplete : MultiplayerCommand {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<SyncTechComplete>();

    private readonly string techId;

    public SyncTechComplete(string techId) {
        this.techId = techId;
    }

    public override void Execute(MultiplayerCommandContext context) {
        if (global::Research.Instance == null)
            return;

        var techInstance = global::Research.Instance.GetTechInstance(techId);
        if (techInstance == null) {
            log.Warning($"Tech {techId} not found; cannot complete on client.");
            return;
        }
        if (techInstance.complete)
            return;

        techInstance.Purchased(); // sets complete = true -> Tech.IsComplete() true -> buildings/items unlock
        if (global::Game.Instance != null)
            global::Game.Instance.Trigger((int) GameHashes.ResearchComplete, techInstance.tech);
    }

}
