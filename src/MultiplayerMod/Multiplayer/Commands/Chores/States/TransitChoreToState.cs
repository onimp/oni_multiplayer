using System;
using System.Collections.Generic;
using System.Linq;
using MultiplayerMod.Game.Chores.States;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.States;

namespace MultiplayerMod.Multiplayer.Commands.Chores.States;

[Serializable]
public class TransitChoreToState : MultiplayerCommand {

    public readonly MultiplayerId ChoreId;
    public readonly string? TargetState;
    public readonly Dictionary<int, object?> Args;

    private TransitChoreToState(MultiplayerId choreId, string? targetState, Dictionary<int, object?> args) {
        ChoreId = choreId;
        TargetState = targetState;
        Args = args;
    }

    public static TransitChoreToState EnterTransition(ChoreTransitStateArgs transitData) =>
        new(
            transitData.Chore.MultiplayerId(),
            transitData.TargetState! + "_" + StatesManager.ContinuationName,
            transitData.Args.ToDictionary(a => a.Key, a => ArgumentUtils.WrapObject(a.Value))
        );

    public static TransitChoreToState ExitTransition(ChoreTransitStateArgs transitData) =>
        new(
            transitData.Chore.MultiplayerId(),
            transitData.TargetState,
            transitData.Args.ToDictionary(a => a.Key, a => ArgumentUtils.WrapObject(a.Value))
        );

    public override void Execute(MultiplayerCommandContext context) {
        var args = Args.ToDictionary(a => a.Key, a => ArgumentUtils.UnWrapObject(a.Value));
        var chore = ChoreObjects.GetChore(ChoreId);
        context.Runtime.Dependencies.Get<StatesManager>().AllowTransition(chore, TargetState, args);
    }
}
