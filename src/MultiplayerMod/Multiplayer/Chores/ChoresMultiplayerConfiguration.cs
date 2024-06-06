using MultiplayerMod.Multiplayer.Chores.Synchronizers;
using static MultiplayerMod.Multiplayer.Chores.Dsl.ChoresConfigurationDsl;

namespace MultiplayerMod.Multiplayer.Chores;

public static class ChoresMultiplayerConfiguration {

    public static readonly ChoreConfiguration[] Configuration = [
        Synchronize<AttackChore>(),
        UseSynchronizer<IdleChore, IdleChore.States, IdleChore.StatesInstance, IdleChoreSynchronizer>(),
        UseSynchronizer<MoveToSafetyChore, MoveToSafetyChore.States, MoveToSafetyChore.StatesInstance, MoveToSafetyChoreSynchronizer>()
    ];

}
