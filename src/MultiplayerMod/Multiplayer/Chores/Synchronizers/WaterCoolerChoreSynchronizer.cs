using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer.StateMachines.Configuration;
using MultiplayerMod.Multiplayer.StateMachines.Configuration.Configurers;

namespace MultiplayerMod.Multiplayer.Chores.Synchronizers;

// Water-cooler socializing. The WaterCooler building offers up to `choreCount` WaterCoolerChores (one per
// valid socialize offset, gated on beverage mass) and duplicants pick them like any errand. Registering a
// synchronizer is enough to fold it into the host-authoritative pipeline: the host's creation replicates
// (CreateChore) + assignment replicates (SetDriverChore), and the client's own building-offered copies are
// cancelled by the ChoresPatcher ctor postfix so only the host-driven one runs.
//
// No state-machine rewrite is needed because the chore is FullyDeterminedByInput: given the same duplicant
// + chat workable it walks to the cooler, plays the drink anim, walks to the chit-chat locator and ToggleWorks
// it for the building's fixed `workTime` (5 s) - every transition is deterministic. The only non-portable
// input is the runtime-spawned `SocialGatheringPointWorkable` (no shared id); ChoreArgumentsWrapper re-resolves
// it to the client's OWN WaterCooler.workables at the same socialize index (the building resolves via shared id).
[Dependency, UsedImplicitly]
public class WaterCoolerChoreSynchronizer
    : ChoreSynchronizer<WaterCoolerChore, WaterCoolerChore.States, WaterCoolerChore.StatesInstance> {

    protected override void Configure(
        IStateMachineRootConfigurer<WaterCoolerChore.States, WaterCoolerChore.StatesInstance, WaterCoolerChore, object> root
    ) {
        // Registration-only: replication + arg resolution (ChoreArgumentsWrapper) fully cover this chore.
    }

}
