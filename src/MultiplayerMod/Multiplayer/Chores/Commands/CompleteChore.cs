using System;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.Chores.Commands;

// Host-authoritative chore completion. The counterpart to CreateChore: the host replicates when a chore is
// created + assigned (CreateChore / SetDriverChore) but nothing told the client when the host's chore ENDED.
// A client chore the host has finished with can loop forever - the client can't always complete it on its
// own (work speed is host-gated, a personal-need chore may wait on unsynced resources) and if the host moves
// on to a chore this client doesn't replicate, no SetDriverChore arrives to stop the old one. Here the host
// broadcasts the chore's shared id the instant its copy cleans up; the client ends its own copy so the
// duplicant's chore lifecycle is bounded by the host's on both ends (CreateChore opens it, CompleteChore
// closes it). No-op in the common case where the client already completed the chore naturally (it's gone
// from the index) or where the next SetDriverChore already stopped it.
[Serializable]
public class CompleteChore : MultiplayerCommand {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<CompleteChore>();

    private readonly MultiplayerId choreId;

    public CompleteChore(MultiplayerId choreId) {
        this.choreId = choreId;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var chore = context.Dependencies.Get<MultiplayerGame>().Objects.Get<Chore>(choreId);
        if (chore == null)
            return; // already ended on this client (natural completion) or never replicated here

        // Cancel ends the chore whether it's mid-execution on a driver or only queued as nextChore: it flows
        // through the chore's own cleanup -> the driver's OnChoreExit -> the driver goes idle and picks its
        // next (host-assigned) chore. Every real side effect is host-authoritative, so cutting the client's
        // copy here only releases the animation and advances the driver; the host's result streams reconcile.
        try {
            chore.Cancel("Host completed chore");
        } catch (Exception exception) {
            log.Warning($"Failed to complete host chore [id={choreId}]: {exception.Message}");
        }
    }

}
