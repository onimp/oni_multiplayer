using System;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Ownership;
using MultiplayerMod.Multiplayer.Players;

namespace MultiplayerMod.Multiplayer.Commands.Player;

/// <summary>
/// Authoritative host-&gt;clients notification that a duplicant's owner changed. Pure applier: it writes the
/// (proxy id -&gt; owner) into the local registry mirror. Carries raw ids only, so it has no dependency on the
/// receiving machine having its player list populated yet - the UI resolves owner names lazily.
/// A null <see cref="owner"/> clears the assignment.
/// </summary>
[Serializable]
public class AssignDuplicantOwner : MultiplayerCommand {

    private readonly MultiplayerId proxyId;
    private readonly PlayerIdentity? owner;

    public AssignDuplicantOwner(MultiplayerId proxyId, PlayerIdentity? owner) {
        this.proxyId = proxyId;
        this.owner = owner;
    }

    public override void Execute(MultiplayerCommandContext context) {
        context.Dependencies.Get<DuplicantOwnershipRegistry>().SetOwner(proxyId, owner);
    }

}
