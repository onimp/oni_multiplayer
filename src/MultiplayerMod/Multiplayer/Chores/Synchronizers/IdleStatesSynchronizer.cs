using JetBrains.Annotations;
using MultiplayerMod.Multiplayer.Commands.Objects;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.Objects.Reference;
using MultiplayerMod.Multiplayer.StateMachines.Commands;
using MultiplayerMod.Multiplayer.StateMachines.Configuration;
using MultiplayerMod.Multiplayer.StateMachines.Configuration.Configurers;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Chores.Synchronizers;

[UsedImplicitly]
public class IdleStatesSynchronizer(
    IMultiplayerServer server,
    MultiplayerObjects objects
) : StateMachineBoundedConfigurer<IdleStates, IdleStates.Instance, IStateMachineTarget, IdleStates.Def> {

    protected override void Configure(
        StateMachineRootConfigurer<IdleStates, IdleStates.Instance, IStateMachineTarget, IdleStates.Def> configurer
    ) {
        configurer.PreConfigure(MultiplayerMode.Client, SetupClient);
        configurer.PostConfigure(MultiplayerMode.Host, SetupHost);
    }

    private void SetupHost(
        StateMachinePostConfigurer<IdleStates, IdleStates.Instance, IStateMachineTarget, IdleStates.Def> configurer
    ) {
        var sm = configurer.StateMachine;

        sm.move.Enter(smi => {
            var target = smi.GetComponent<Navigator>().targetLocator;
            var cell = Grid.PosToCell(target);

            // TODO: Remove after critters sync (WorldGenSpawner.Spawnable + new critters)
            if (objects.Get(smi.master.gameObject) == null)
                return;

            server.Send(
                new MoveObjectToCell(new StateMachineReference(smi.controller.GetReference(), smi.GetType()), cell, sm.move),
                MultiplayerCommandOptions.SkipHost
            );
        });
        sm.move.Exit(smi => {

            // TODO: Remove after critters sync (WorldGenSpawner.Spawnable + new critters)
            if (objects.Get(smi.master.gameObject) == null)
                return;

            server.Send(
                new GoToState(new StateMachineReference(smi.controller.GetReference(), smi.GetType()), sm.loop),
                MultiplayerCommandOptions.SkipHost
            );
            server.Send(
                new SynchronizeObjectPosition(smi.gameObject),
                MultiplayerCommandOptions.SkipHost
            );
        });
    }

    private void SetupClient(
        StateMachinePreConfigurer<IdleStates, IdleStates.Instance, IStateMachineTarget, IdleStates.Def> configurer
    ) {
        var sm = configurer.StateMachine;

        // Suppress transitions to the "move" state
        configurer.Suppress(() => sm.loop.ToggleScheduleCallback(null, null, null));

        // Do nothing on move state
        configurer.Suppress(() => sm.move.Enter(null));

        // Configure "move" to move to the synchronized cell
        configurer.PostConfigure(post => {
            var targetCell = post.AddMultiplayerParameter(MoveObjectToCell.TargetCell);
            sm.move.Enter(smi => {
                var navigator = smi.GetComponent<Navigator>();
                navigator.GoTo(targetCell.Get(smi));
            });
        });
    }

}
