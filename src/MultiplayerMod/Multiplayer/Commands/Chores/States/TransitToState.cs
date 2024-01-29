using System;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.States;

namespace MultiplayerMod.Multiplayer.Commands.Chores.States;

[Serializable]
public class TransitToState : MultiplayerCommand {
    public readonly MultiplayerId ChoreId;
    public readonly string? State;

    public TransitToState(Chore chore, string? state) {
        ChoreId = chore.MultiplayerId();
        State = state;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var chore = ChoreObjects.GetChore(ChoreId);
        var smi = context.Runtime.Dependencies.Get<StatesManager>().GetSmi(chore);
        smi.GoTo(State);
    }
}
