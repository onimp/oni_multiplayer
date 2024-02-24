using System;
using MultiplayerMod.Core.Reflection;
using MultiplayerMod.Game.Chores.States;
using MultiplayerMod.Game.NameOf;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.States;

namespace MultiplayerMod.Multiplayer.Commands.Chores.States;

[Serializable]
public class UpdateMove : MultiplayerCommand {
    public readonly MultiplayerId ChoreId;
    public readonly int Cell;

    public UpdateMove(MoveToArgs args) {
        ChoreId = args.Chore.MultiplayerId();
        Cell = args.Cell;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var chore = ChoreObjects.GetChore(ChoreId);
        var smi = context.Runtime.Dependencies.Get<StatesManager>().GetSmi(chore);

        var sm = smi.stateMachine;
        var target = sm.GetFieldValue("stateTarget");
        var navigator = (Navigator) target.GetType().GetMethod(nameof(StateMachineMemberReference.TargetParameter.Get))!
            .MakeGenericMethod(typeof(Navigator))
            .Invoke(target, new object[] { smi });
        navigator.UpdateTarget(Cell);
    }
}
