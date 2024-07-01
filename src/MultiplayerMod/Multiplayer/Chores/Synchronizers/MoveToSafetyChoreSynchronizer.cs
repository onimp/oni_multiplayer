using JetBrains.Annotations;
using MultiplayerMod.Multiplayer.Commands.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;
using MultiplayerMod.Multiplayer.StateMachines.Commands;
using MultiplayerMod.Multiplayer.StateMachines.Configuration;
using MultiplayerMod.Multiplayer.StateMachines.Configuration.Configurers;
using MultiplayerMod.Multiplayer.StateMachines.Configuration.States;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Chores.Synchronizers;

[UsedImplicitly]
public class MoveToSafetyChoreSynchronizer(IMultiplayerServer server) : StateMachineBoundedConfigurer<
    MoveToSafetyChore.States,
    MoveToSafetyChore.StatesInstance,
    MoveToSafetyChore
> {

    private readonly StateMachineMultiplayerStateInfo movingStateInfo = new(name: "__moving");

    protected override StateMachineConfigurer[] Inline() => [
        // Disable IdleChore recurring creation
        new StateMachineConfigurerDsl<SafeCellMonitor, SafeCellMonitor.Instance>(root => {
            root.PreConfigure(MultiplayerMode.Client, pre => {
                pre.Suppress(() => pre.StateMachine.danger.ToggleChore(null, null));
            });
        })
    ];

    protected override void Configure(
        StateMachineRootConfigurer<MoveToSafetyChore.States, MoveToSafetyChore.StatesInstance, MoveToSafetyChore, object> configurer
    ) {
        configurer.PostConfigure(MultiplayerMode.Host, SetupHost);
        configurer.PostConfigure(MultiplayerMode.Client, SetupClient);
    }


    private void SetupClient(
        StateMachinePostConfigurer<MoveToSafetyChore.States, MoveToSafetyChore.StatesInstance, MoveToSafetyChore, object> configurer
    ) {
        var sm = configurer.StateMachine;
        var waiting = configurer.AddState(sm.root, "__waiting");
        var moving = configurer.AddState(sm.root, movingStateInfo);

        var targetCell = configurer.AddMultiplayerParameter(MoveObjectToCell.TargetCell);

        moving.MoveTo(smi => targetCell.Get(smi), waiting, waiting, true);

        // Redirect default state to the waiting state
        sm.root.Enter(smi => smi.GoTo(waiting));
    }

    private void SetupHost(
        StateMachinePostConfigurer<MoveToSafetyChore.States, MoveToSafetyChore.StatesInstance, MoveToSafetyChore, object> configurer
    ) {
        var sm = configurer.StateMachine;
        sm.move.Enter(smi => {
            server.Send(
                new MoveObjectToCell(new ChoreStateMachineReference(smi.master), smi.targetCell, movingStateInfo),
                MultiplayerCommandOptions.SkipHost
            );
        });

        sm.move.Update((smi, _) => {
            server.Send(
                new MoveObjectToCell(new ChoreStateMachineReference(smi.master), smi.targetCell, movingStateInfo),
                MultiplayerCommandOptions.SkipHost
            );
        });

        sm.move.Exit(smi => {
            server.Send(
                new GoToState(new ChoreStateMachineReference(smi.master), (StateMachine.BaseState?) null),
                MultiplayerCommandOptions.SkipHost
            );
            server.Send(
                new SynchronizeObjectPosition(smi.gameObject),
                MultiplayerCommandOptions.SkipHost
            );
        });

    }

}
