using System;
using System.Collections.Generic;
using MultiplayerMod.Multiplayer.Ownership;
using MultiplayerMod.Multiplayer.Players;

namespace MultiplayerMod.Multiplayer.Commands.Player;

/// <summary>
/// Sent by a client after it issues a work order (dig/build), carrying the cells it ordered and its own
/// player id. Runs on the host (via the loopback client, so <c>ClientId</c> is null — hence the owner is
/// embedded, not derived) and tags the work targets at those cells with the ordering player.
///
/// Ordering: the client sends this immediately after the order command on the same reliable lane, so the
/// targets already exist on the host when this runs. If they don't (out-of-order/edge case), tagging is a
/// harmless no-op and the chore simply stays unowned (globally available).
/// </summary>
[Serializable]
public class TagChoreTargets : MultiplayerCommand {

    private readonly List<int> cells;
    private readonly PlayerIdentity owner;

    public TagChoreTargets(List<int> cells, PlayerIdentity owner) {
        this.cells = cells;
        this.owner = owner;
    }

    public override void Execute(MultiplayerCommandContext context) {
        if (context.Multiplayer.Mode != MultiplayerMode.Host)
            return;
        context.Dependencies.Get<ChoreOwnershipRegistry>().TagCells(cells, owner);
    }

}
