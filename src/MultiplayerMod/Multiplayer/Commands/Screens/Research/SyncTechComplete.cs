using System;
using MultiplayerMod.Core.Logging;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Research;

// Host-authoritative research unlock (see ResearchCompletionSynchronizer). The research-point sim runs
// independently on each machine and is otherwise only reconciled at the daily hard-sync - and on the
// client it barely accrues at all, since its research work is host-gated (WorkChoreSynchronizer). Client
// self-completion is also suppressed outright (ResearchCompletionSynchronizer skips CheckBuyResearch on
// the client), so this command is the SOLE driver of research completion on the client.
//
// We replicate the whole tail of Research.CheckBuyResearch, not just the unlock: mark the client's
// TechInstance complete (which is what makes the tech's buildings/items available), fire ResearchComplete
// so notifications / subscribers update, and - crucially - advance the queue via GetNextTech() so the
// finished tech is cleared from the active slot and the next queued tech starts, exactly as it does on the
// host. Without that advance the client stays pinned to the (now complete) active tech and never moves on.
// Idempotent - a tech already complete on the client is left alone.
[Serializable]
public class SyncTechComplete : MultiplayerCommand {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<SyncTechComplete>();

    private readonly string techId;

    public SyncTechComplete(string techId) {
        this.techId = techId;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var research = global::Research.Instance;
        if (research == null)
            return;

        var techInstance = research.GetTechInstance(techId);
        if (techInstance == null) {
            log.Warning($"Tech {techId} not found; cannot complete on client.");
            return;
        }
        if (techInstance.complete)
            return;

        techInstance.Purchased(); // sets complete = true -> Tech.IsComplete() true -> buildings/items unlock
        if (global::Game.Instance != null)
            global::Game.Instance.Trigger((int) GameHashes.ResearchComplete, techInstance.tech);

        // Mirror the queue advance that Research.CheckBuyResearch does right after Purchased(): only when the
        // finished tech is this client's active research (the normal case - selection is synced). GetNextTech()
        // removes queuedTech[0], activates the next queued tech (or clears active), and re-notifies the research
        // screen / centers. Its inner CheckBuyResearch is a no-op on the client (suppressed in
        // ResearchCompletionSynchronizer), so activating the next tech can't cascade into another local purchase.
        // If the finished tech isn't the active one (queue divergence), leaving it Purchased() is enough - the
        // unlock still stands and there's nothing in the active slot to advance.
        if (research.GetActiveResearch() == techInstance)
            research.GetNextTech();
    }

}
