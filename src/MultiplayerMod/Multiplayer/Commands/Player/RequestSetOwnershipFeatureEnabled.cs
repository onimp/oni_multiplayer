using System;
using MultiplayerMod.Multiplayer.Ownership;

namespace MultiplayerMod.Multiplayer.Commands.Player;

/// <summary>
/// Request to toggle the assignment feature. Executes on the host, which applies it authoritatively and
/// broadcasts <see cref="SetOwnershipFeatureEnabled"/>. Disabling is a host-only affordance in the UI; this
/// request path exists so the host's own management screen has a single code path with the client mirror.
/// </summary>
[Serializable]
[MultiplayerCommand(Type = MultiplayerCommandType.System, ExecuteOnServer = true)]
public class RequestSetOwnershipFeatureEnabled : MultiplayerCommand {

    private readonly bool enabled;

    public RequestSetOwnershipFeatureEnabled(bool enabled) {
        this.enabled = enabled;
    }

    public override void Execute(MultiplayerCommandContext context) {
        context.Dependencies.Get<DuplicantOwnershipController>().SetEnabled(enabled);
    }

}
