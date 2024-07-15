using MultiplayerMod.Multiplayer.Chores.Synchronizers;
using MultiplayerMod.Multiplayer.StateMachines.Configuration;
using static MultiplayerMod.Multiplayer.Chores.Dsl.ChoresConfigurationDsl;

namespace MultiplayerMod.Multiplayer.Chores;

public static class ChoresMultiplayerConfiguration {

    public static readonly ChoreConfiguration[] Configuration = [
        // @formatter:off
        Synchronize<AttackChore, AttackChore.States, AttackChore.StatesInstance>(root => {
            root.Inline(new StateMachineConfigurerDsl<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>(configurer => {
                configurer.PreConfigure(MultiplayerMode.Client, pre => {
                    pre.Suppress(() => pre.StateMachine.threatened.duplicant.ShouldFight.ToggleChore(null, null));
                });
            }));
        }),
        UseSynchronizer<IdleChore, IdleChore.States, IdleChore.StatesInstance, IdleChoreSynchronizer>(),
        UseSynchronizer<MoveToSafetyChore, MoveToSafetyChore.States, MoveToSafetyChore.StatesInstance, MoveToSafetyChoreSynchronizer>(),
        UseStateMachineSynchronizer<IdleStates, IdleStates.Instance, IdleStates.Def, IdleStatesSynchronizer>()
        // DieChore and DeathMonitor are synchronized on the upper level (see DeathMonitorSynchronizer).

        // @formatter:on
    ];

}
