using System;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Ownership;
using MultiplayerMod.Multiplayer.Players;

namespace MultiplayerMod.Multiplayer.Commands.Player;

/// <summary>
/// Client-&gt;host request to (re)assign a duplicant's owner. Executes on the host, which applies it to the
/// authoritative registry and fans an <see cref="AssignDuplicantOwner"/> back out to every client (including
/// the requester). A null <see cref="owner"/> clears the assignment.
/// </summary>
[Serializable]
[MultiplayerCommand(Type = MultiplayerCommandType.System, ExecuteOnServer = true)]
public class RequestAssignDuplicantOwner : MultiplayerCommand {

    private readonly MultiplayerId proxyId;
    private readonly PlayerIdentity? owner;

    public RequestAssignDuplicantOwner(MultiplayerId proxyId, PlayerIdentity? owner) {
        this.proxyId = proxyId;
        this.owner = owner;
    }

    public override void Execute(MultiplayerCommandContext context) {
        context.Dependencies.Get<DuplicantOwnershipController>().Assign(proxyId, owner);
    }

}
