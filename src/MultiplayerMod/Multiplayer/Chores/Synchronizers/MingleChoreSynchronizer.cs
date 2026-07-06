using System;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer.Commands.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;
using MultiplayerMod.Multiplayer.StateMachines.Commands;
using MultiplayerMod.Multiplayer.StateMachines.Configuration;
using MultiplayerMod.Multiplayer.StateMachines.Configuration.Configurers;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Chores.Synchronizers;

// Mingle is "Idle during Recreation": MingleMonitor recurringly spawns a MingleChore whose only ctor
// arg is the duplicant, so it replicates cleanly through the standard chore-creation path. The
// non-deterministic parts are the chosen mingle cell (MingleCellSensor.GetCell, picked randomly among
// the closest rec-room cells) and the random onfloor dwell (ScheduleGoTo(Random.Range(5,10))). We drive
// the client from the host exactly like IdleChoreSynchronizer: the host tells the client which cell to
// walk to and lets the client's own navigation land it in `onfloor`, where it loops the conversation
// animation until the host's chore completes and ReleaseChoreDriver tears the client copy down in
// lockstep. Rec-time gating is free: the schedule is already synced and the client copy is host-driven.
[Dependency, UsedImplicitly]
public class MingleChoreSynchronizer(IMultiplayerServer server)
    : ChoreSynchronizer<MingleChore, MingleChore.States, MingleChore.StatesInstance> {

    protected override void Configure(
        IStateMachineRootConfigurer<MingleChore.States, MingleChore.StatesInstance, MingleChore, object> root
    ) {
        // Disable the client-side recurring mingle chore creation; only the host-replicated copy exists.
        root.Inline(new StateMachineConfigurerDsl<MingleMonitor, MingleMonitor.Instance>(monitor => {
            monitor.PreConfigure(MultiplayerMode.Client, pre => {
                pre.Suppress(() => pre.StateMachine.mingle.ToggleRecurringChore(null, null));
            });
        }));

        root.PreConfigure(MultiplayerMode.Host, SetupHost);
        root.PreConfigure(MultiplayerMode.Client, SetupClient);
    }

    private void SetupClient(
        StateMachinePreConfigurer<MingleChore.States, MingleChore.StatesInstance, MingleChore, object> configurer
    ) {
        var sm = configurer.StateMachine;

        // The client is driven entirely by host MoveObjectToCell commands, so strip its local,
        // non-deterministic room/cell decisions (IsSameRoom → move/walk, GetMingleCell) and the random
        // self-timeout out of `onfloor`. The conversation anim + AlwaysConverse tag on `onfloor` stay.
        configurer.Suppress(() => sm.mingle.Transition(null, null, 0));
        configurer.Suppress(() => sm.move.Transition(null, null, 0));
        configurer.Suppress(() => sm.move.MoveTo(null, null, null, false));
        configurer.Suppress(() => sm.walk.Transition(null, null, 0));
        configurer.Suppress(() => sm.walk.MoveTo(null, null, null, false));
        configurer.Suppress(() => sm.onfloor.ScheduleGoTo((Func<MingleChore.StatesInstance, float>) null!, null));

        // Re-point both movement states at the host-synchronized cell; whichever one the host entered
        // (move for cross-room, walk for same-room) navigates there and lands in `onfloor`.
        configurer.PostConfigure(post => {
            var targetCell = post.AddMultiplayerParameter(MoveObjectToCell.TargetCell);
            sm.move.MoveTo(smi => targetCell.Get(smi), sm.onfloor, null);
            sm.walk.MoveTo(smi => targetCell.Get(smi), sm.onfloor, null);
        });
    }

    private void SetupHost(
        StateMachinePreConfigurer<MingleChore.States, MingleChore.StatesInstance, MingleChore, object> configurer
    ) {
        var sm = configurer.StateMachine;

        sm.move.Enter(smi => server.Send(
            new MoveObjectToCell(new ChoreStateMachineReference(smi.master), smi.GetMingleCell(), sm.move)
        ));
        sm.walk.Enter(smi => server.Send(
            new MoveObjectToCell(new ChoreStateMachineReference(smi.master), smi.GetMingleCell(), sm.walk)
        ));

        // Snap the client to the exact mingle cell once the host arrives and begins conversing.
        sm.onfloor.Enter(smi => server.Send(new SynchronizeObjectPosition(smi.gameObject)));
    }

}
