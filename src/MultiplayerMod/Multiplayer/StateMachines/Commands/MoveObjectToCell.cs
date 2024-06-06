using System;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Objects.Reference;
using MultiplayerMod.Multiplayer.StateMachines.Configuration.Parameters;
using MultiplayerMod.Multiplayer.StateMachines.RuntimeTools;

namespace MultiplayerMod.Multiplayer.StateMachines.Commands;

[Serializable]
public class MoveObjectToCell : MultiplayerCommand {

    public static StateMachineMultiplayerParameterInfo<int> TargetCell = new(
        "__move_to_target_cell",
        defaultValue: Grid.InvalidCell
    );

    private readonly Reference<StateMachine.Instance> reference;
    private readonly string? movingStateName;
    private readonly int cell;

    public MoveObjectToCell(Reference<StateMachine.Instance> reference, int cell, StateMachine.BaseState? movingState) :
        this(reference, cell, movingState?.name) { }

    public MoveObjectToCell(Reference<StateMachine.Instance> reference, int cell, string? movingStateName) {
        this.movingStateName = movingStateName;
        this.reference = reference;
        this.cell = cell;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var runtime = StateMachineRuntimeTools.Get(reference.Resolve());
        runtime.FindParameter(TargetCell)?.Set(cell);
        runtime.GoToState(movingStateName);
    }

}
