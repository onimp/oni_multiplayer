using System;
using MultiplayerMod.Multiplayer.Ownership;

namespace MultiplayerMod.Multiplayer.Commands.Player;

/// <summary>
/// Authoritative host-&gt;clients notification that the per-player duplicant-assignment feature was toggled.
/// When disabled the whole system goes inert (vanilla behaviour); see <see cref="DuplicantOwnershipRegistry"/>.
/// </summary>
[Serializable]
public class SetOwnershipFeatureEnabled : MultiplayerCommand {

    private readonly bool enabled;

    public SetOwnershipFeatureEnabled(bool enabled) {
        this.enabled = enabled;
    }

    public override void Execute(MultiplayerCommandContext context) {
        context.Dependencies.Get<DuplicantOwnershipRegistry>().SetEnabled(enabled);
    }

}
