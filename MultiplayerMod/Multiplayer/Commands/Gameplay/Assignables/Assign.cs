using System;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay.Assignables;

[Serializable]
public class Assign : IMultiplayerCommand {

    private MultiplayerReference Target { get; }
    private MultiplayerReference Identity { get; }

    public Assign(MultiplayerReference target, MultiplayerReference identity) {
        Target = target;
        Identity = identity;
    }

    public void Execute() {
        var assignable = Target.GetComponent<Assignable>();
        var identity = Identity?.GetComponent<IAssignableIdentity>();
        if (identity != null)
            assignable.Assign(identity);
        else
            assignable.Unassign();
    }

}
