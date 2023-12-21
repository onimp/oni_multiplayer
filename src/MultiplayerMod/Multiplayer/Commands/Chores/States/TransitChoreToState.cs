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
    public readonly Dictionary<int, object> Args;

    public TransitChoreToState(ChoreTransitStateArgs transitData) {
        ChoreId = transitData.ChoreId;
        TargetState = transitData.TargetState;
        Args = transitData.Args.ToDictionary(a => a.Key, a => ArgumentUtils.WrapObject(a.Value)!);
    }

    public override void Execute(MultiplayerCommandContext context) {
        var args = Args.ToDictionary(a => a.Key, a => ArgumentUtils.UnWrapObject(a.Value)!);
        context.Runtime.Dependencies.Get<StatesManager>().AllowTransition(ChoreId, TargetState, args);
    }
}
