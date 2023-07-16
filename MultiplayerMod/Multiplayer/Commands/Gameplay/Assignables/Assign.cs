using System;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay.Assignables;

[Serializable]
public class Assign : IMultiplayerCommand {

    private GameObjectReference Target { get; }
    private GameObjectReference? Identity { get; }

    public Assign(GameObjectReference target, GameObjectReference? identity) {
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
