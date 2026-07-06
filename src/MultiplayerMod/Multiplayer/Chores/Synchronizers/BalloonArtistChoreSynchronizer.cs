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

// Balloon artist joy reaction: an Overjoyed duplicant with the trait walks to a stand cell, spawns a balloon
// stand and hands balloons to passers-by. The BalloonArtist monitor recurringly spawns a BalloonArtistChore
// (ctor arg is only the duplicant, so it replicates cleanly through the standard creation path). We suppress the
// client's own recurring creation and let the host-replicated copy drive the same dupe.
//
// The one non-deterministic step is the stand cell (BalloonStandCellSensor picks among valid room cells), so we
// drive it host->client exactly like Idle/Mingle: strip the client's own cell pick + fail transition and re-point
// `goToStand` at the host-synchronized cell; the host emits a MoveObjectToCell on `goToStand` entry. Everything
// after (spawn stand, play working loop, give balloons) runs locally on both sides - the balloon *items* handed
// out are materials and fall to hard-sync, but the artist-at-stand animation tracks the host.
[Dependency, UsedImplicitly]
public class BalloonArtistChoreSynchronizer(IMultiplayerServer server)
    : ChoreSynchronizer<BalloonArtistChore, BalloonArtistChore.States, BalloonArtistChore.StatesInstance> {

    protected override void Configure(
        IStateMachineRootConfigurer<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object> root
    ) {
        // Disable the client-side recurring balloon-artist chore creation; only the host-replicated copy exists.
        root.Inline(new StateMachineConfigurerDsl<BalloonArtist, BalloonArtist.Instance>(monitor => {
            monitor.PreConfigure(MultiplayerMode.Client, pre => {
                pre.Suppress(() => pre.StateMachine.overjoyed.balloon_stand.ToggleChore(
                    (Func<BalloonArtist.Instance, Chore>) null!, null
                ));
            });
        }));

        root.PreConfigure(MultiplayerMode.Host, SetupHost);
        root.PreConfigure(MultiplayerMode.Client, SetupClient);
    }

    private void SetupClient(
        StateMachinePreConfigurer<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object> configurer
    ) {
        var sm = configurer.StateMachine;

        // Strip the client's own stand-cell decision: the fail transition (no stall cell) and the local MoveTo,
        // then re-point goToStand at the host-synchronized cell, which navigates it into balloonStand.
        configurer.Suppress(() => sm.goToStand.Transition(null, null, 0));
        configurer.Suppress(() => sm.goToStand.MoveTo(null, null, null, false));

        configurer.PostConfigure(post => {
            var targetCell = post.AddMultiplayerParameter(MoveObjectToCell.TargetCell);
            sm.goToStand.MoveTo(smi => targetCell.Get(smi), sm.balloonStand, null);
        });
    }

    private void SetupHost(
        StateMachinePreConfigurer<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object> configurer
    ) {
        var sm = configurer.StateMachine;

        sm.goToStand.Enter(smi => {
            if (!smi.HasBalloonStallCell()) // inits the sensor; if no cell the host chore fails as in vanilla
                return;
            server.Send(new MoveObjectToCell(
                new ChoreStateMachineReference(smi.master), smi.GetBalloonStallCell(), sm.goToStand
            ));
        });

        // Snap the client onto the exact stand cell once the host arrives and starts handing out balloons.
        sm.goToStand.Exit(smi => server.Send(new SynchronizeObjectPosition(smi.gameObject)));
    }

}
