using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer.Commands.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;
using MultiplayerMod.Multiplayer.StateMachines.Commands;
using MultiplayerMod.Multiplayer.StateMachines.Configuration;
using MultiplayerMod.Multiplayer.StateMachines.Configuration.Configurers;
using MultiplayerMod.Multiplayer.StateMachines.Configuration.States;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Chores.Synchronizers;

[Dependency, UsedImplicitly]
public class MoveToSafetyChoreSynchronizer(
    IMultiplayerServer server
) : ChoreSynchronizer<MoveToSafetyChore, MoveToSafetyChore.States, MoveToSafetyChore.StatesInstance> {

    private readonly StateMachineMultiplayerStateInfo movingStateInfo = new(name: "__moving");

    protected override void Configure(IStateMachineRootConfigurer<MoveToSafetyChore.States, MoveToSafetyChore.StatesInstance, MoveToSafetyChore, object> root) {
        // Disable IdleChore recurring creation
        root.Inline(new StateMachineConfigurerDsl<SafeCellMonitor, SafeCellMonitor.Instance, IStateMachineTarget, SafeCellMonitor.Def>(monitor => {
            monitor.PreConfigure(MultiplayerMode.Client, pre => {
                pre.Suppress(() => pre.StateMachine.danger.ToggleChore(null, null));
            });
        }));

        root.PostConfigure(MultiplayerMode.Host, SetupHost);
        root.PostConfigure(MultiplayerMode.Client, SetupClient);
    }

    private void SetupClient(StateMachinePostConfigurer<MoveToSafetyChore.States, MoveToSafetyChore.StatesInstance, MoveToSafetyChore, object> configurer) {
        var sm = configurer.StateMachine;
        var waiting = configurer.AddState(sm.root, "__waiting");
        var moving = configurer.AddState(sm.root, movingStateInfo);

        var targetCell = configurer.AddMultiplayerParameter(MoveObjectToCell.TargetCell);

        moving.MoveTo(smi => targetCell.Get(smi), waiting, waiting, true);

        // Redirect default state to the waiting state
        sm.root.Enter(smi => smi.GoTo(waiting));
    }

    private void SetupHost(StateMachinePostConfigurer<MoveToSafetyChore.States, MoveToSafetyChore.StatesInstance, MoveToSafetyChore, object> configurer) {
        var sm = configurer.StateMachine;

        sm.move.Enter(smi => {
            var reference = new ChoreStateMachineReference(smi.master);
            server.Send(new MoveObjectToCell(reference, smi.targetCell, movingStateInfo));
        });

        sm.move.Update((smi, _) => {
            var reference = new ChoreStateMachineReference(smi.master);
            server.Send(new MoveObjectToCell(reference, smi.targetCell, movingStateInfo));
        });

        sm.move.Exit(smi => {
            var reference = new ChoreStateMachineReference(smi.master);
            server.Send(new GoToState(reference, (StateMachine.BaseState?) null));
            server.Send(new SynchronizeObjectPosition(smi.gameObject));
        });
    }

}
