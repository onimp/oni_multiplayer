using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer.StateMachines.Configuration;
using MultiplayerMod.Multiplayer.StateMachines.Configuration.Configurers;

namespace MultiplayerMod.Multiplayer.Chores.Synchronizers;

// Room parties. When a rec room throws a party the game spawns a throw-away "PartyWorkable" locator at a random
// cell in the room with a random work time, then offers a PartyChore anchored to that locator; duplicants pick
// it and gather to chat. Registering a synchronizer replicates the host's creation (CreateChore) + assignment
// (SetDriverChore) and cancels the client's own copy via the ChoresPatcher ctor postfix.
//
// Like WaterCooler the chore is FullyDeterminedByInput, so no state-machine rewrite is needed - but its master
// AND chat workable are the same runtime locator, which has no shared id and sits at a random cell with a random
// work time. ChoreArgumentsWrapper ships the cell + work time and rebuilds an equivalent locator on the client
// (mirroring the host spawn) so both sides gather at the same spot for the same duration; the rebuilt locator is
// torn down by a client-side on_end callback when the chore ends, so nothing leaks.
[Dependency, UsedImplicitly]
public class PartyChoreSynchronizer
    : ChoreSynchronizer<PartyChore, PartyChore.States, PartyChore.StatesInstance> {

    protected override void Configure(
        IStateMachineRootConfigurer<PartyChore.States, PartyChore.StatesInstance, PartyChore, object> root
    ) {
        // Registration-only: replication + locator rebuild (ChoreArgumentsWrapper) fully cover this chore.
    }

}
