using System;
using MultiplayerMod.Core.Reflection;
using MultiplayerMod.Game.Chores.States;
using MultiplayerMod.Game.NameOf;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.States;

namespace MultiplayerMod.Multiplayer.Chores.Commands.States;

[Serializable]
public class StartMove : MultiplayerCommand {
    public readonly MultiplayerId ChoreId;
    public readonly string? TargetState;
    public readonly int Cell;
    public readonly CellOffset[] Offsets;

    public StartMove(MoveToArgs args) {
        ChoreId = args.Chore.MultiplayerId();
        TargetState = args.TargetState;
        Cell = args.Cell;
        Offsets = args.Offsets;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var chore = context.Multiplayer.Objects.Get<Chore>(ChoreId)!;
        var smi = context.Runtime.Dependencies.Get<StatesManager>().GetSmi(chore);

        smi.GoTo("root." + TargetState);

        var sm = smi.stateMachine;
        var target = sm.GetFieldValue("stateTarget");
        var navigator = (Navigator) target.GetType().GetMethod(nameof(StateMachine<StateMachineMemberReference, StateMachineMemberReference.Instance, KMonoBehaviour, object>.TargetParameter.Get))!
            .MakeGenericMethod(typeof(Navigator))
            .Invoke(target, new object[] { smi });
        navigator.GoTo(Cell, Offsets);
    }
}
