using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer.StateMachines.Configuration;
using MultiplayerMod.Multiplayer.StateMachines.Configuration.Configurers;

namespace MultiplayerMod.Multiplayer.Chores.Synchronizers;

// Eat is a personal-need chore, structurally identical to Pee (a need routed to a building), so it syncs
// the same minimal way: suppress the client-side monitor that recurringly spawns the chore, and let the
// standard chore-creation path replicate the host's EatChore (ctor arg is only the duplicant) and assign
// it to the same dupe. The client then runs the replicated chore against its own local resources - it
// picks a mess table / edible from its own world exactly as PeeChore picks a toilet - so we do NOT force
// the host's exact table or the exact floor-eat cell (those need shared building identity the client
// doesn't have mid-cycle).
//
// Best-effort boundary: EatChore.Begin resolves the edible from the client's own RationMonitor. The
// materials economy is not synced, so if the client happens to have no edible the chore assignment throws
// in Begin and is caught + skipped by the command handler; that dupe reconciles at the next hard-sync.
// In the common case the client still holds (stale) food and eats normally.
[Dependency, UsedImplicitly]
public class EatChoreSynchronizer : ChoreSynchronizer<EatChore, EatChore.States, EatChore.StatesInstance> {

    protected override void Configure(
        IStateMachineRootConfigurer<EatChore.States, EatChore.StatesInstance, EatChore, object> root
    ) {
        root.Inline(new StateMachineConfigurerDsl<RationMonitor, RationMonitor.Instance>(monitor => {
            monitor.PreConfigure(MultiplayerMode.Client, pre =>
                pre.Suppress(() => pre.StateMachine.rationsavailable.edibleavailable.ToggleChore(null, null))
            );
        }));
    }

}
