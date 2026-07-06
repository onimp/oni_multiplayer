using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer.StateMachines.Configuration;
using MultiplayerMod.Multiplayer.StateMachines.Configuration.Configurers;
// Disambiguates ToggleChore(Func, State, State) from ToggleChore(Func, Action, State) in the Suppress
// expressions below (the monitor uses the (Func, State, State) overload).
using SleepMonitorState =
    GameStateMachine<SleepChoreMonitor, SleepChoreMonitor.Instance, IStateMachineTarget, object>.State;

namespace MultiplayerMod.Multiplayer.Chores.Synchronizers;

// Sleep is a personal-need chore like Pee/Eat, so it syncs the same minimal way: suppress the four
// SleepChoreMonitor states that recurringly spawn a SleepChore on the client, and let the standard
// chore-creation path replicate the host's SleepChore and assign it to the same dupe.
//
// Sleep differs from Eat in one way that sets its best-effort boundary: SleepChore takes the `bed` in its
// constructor (Eat resolves its target at runtime), so CreateChore must resolve that bed reference on the
// client. An assigned bed building present since the last hard-sync carries a shared MultiplayerId and
// resolves fine, so a dupe sleeping in its own bed replicates. Ephemeral floor/passed-out sleep locators
// and beds built mid-cycle have no shared identity: their reference degrades to a grid-cell reference that
// resolves to nothing (or the wrong object) on the client, so the chore fails harmlessly in Begin and is
// caught + skipped by the command handler - that dupe reconciles at the next hard-sync. The rare
// "exhausted" collapse uses the second SleepChore constructor (with a StatusItem[]) that ChoresPatcher
// does not patch, so it also falls back.
[Dependency, UsedImplicitly]
public class SleepChoreSynchronizer : ChoreSynchronizer<SleepChore, SleepChore.States, SleepChore.StatesInstance> {

    protected override void Configure(
        IStateMachineRootConfigurer<SleepChore.States, SleepChore.StatesInstance, SleepChore, object> root
    ) {
        root.Inline(new StateMachineConfigurerDsl<SleepChoreMonitor, SleepChoreMonitor.Instance>(monitor => {
            monitor.PreConfigure(MultiplayerMode.Client, pre => {
                pre.Suppress(() => pre.StateMachine.passingout.ToggleChore(null, (SleepMonitorState) null!, null));
                pre.Suppress(() => pre.StateMachine.passingout_bedassigned.ToggleChore(null, (SleepMonitorState) null!, null));
                pre.Suppress(() => pre.StateMachine.sleeponfloor.ToggleChore(null, (SleepMonitorState) null!, null));
                pre.Suppress(() => pre.StateMachine.bedassigned.ToggleChore(null, (SleepMonitorState) null!, null));
            });
        }));
    }

}
