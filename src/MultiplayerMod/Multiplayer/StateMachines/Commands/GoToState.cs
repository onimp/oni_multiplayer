using System;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Objects.Reference;
using MultiplayerMod.Multiplayer.StateMachines.RuntimeTools;

namespace MultiplayerMod.Multiplayer.StateMachines.Commands;

[Serializable]
public class GoToState : MultiplayerCommand {

    private readonly Reference<StateMachine.Instance> reference;
    private readonly string? stateName;

    public GoToState(Reference<StateMachine.Instance> reference, StateMachine.BaseState? state) : this(
        reference,
        state?.name
    ) { }

    public GoToState(Reference<StateMachine.Instance> reference, string? stateName) {
        this.stateName = stateName;
        this.reference = reference;
    }

    public override void Execute(MultiplayerCommandContext context) {
        StateMachineRuntimeTools.Get(reference.Resolve()).GoToState(stateName);
    }

}
