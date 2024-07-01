using System;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.States;

namespace MultiplayerMod.Multiplayer.Chores.Commands.States;

[Serializable]
public class TransitToState : MultiplayerCommand {
    public readonly MultiplayerId ChoreId;
    public readonly string? State;

    public TransitToState(Chore chore, string? state) {
        ChoreId = chore.MultiplayerId();
        State = state;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var chore = context.Multiplayer.Objects.Get<Chore>(ChoreId)!;
        var smi = context.Runtime.Dependencies.Get<StatesManager>().GetSmi(chore);
        smi.GoTo(State);
    }
}
