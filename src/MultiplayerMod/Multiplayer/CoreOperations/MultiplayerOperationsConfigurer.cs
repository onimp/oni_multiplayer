using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer.CoreOperations.Binders;
using MultiplayerMod.Multiplayer.CoreOperations.CommandExecution;
using MultiplayerMod.Multiplayer.CoreOperations.PlayersManagement;

namespace MultiplayerMod.Multiplayer.CoreOperations;

[UsedImplicitly]
public class MultiplayerOperationsConfigurer {

    private readonly DependencyContainer dependencies;

    public MultiplayerOperationsConfigurer(DependencyContainer dependencies) {
        this.dependencies = dependencies;
    }

    public void Configure() {
        dependencies.Register<MultiplayerCommandExecutor>();
        dependencies.Register<GameEventsBinder>(DependencyOptions.AutoResolve);
        dependencies.Register<HostEventsBinder>(DependencyOptions.AutoResolve);
        dependencies.Register<GameStateEventsRelay>(DependencyOptions.AutoResolve);
        dependencies.Register<ExecutionLevelController>(DependencyOptions.AutoResolve);
        dependencies.Register<MultiplayerServerController>(DependencyOptions.AutoResolve);
        dependencies.Register<MultiplayerGameObjectsSpawner>(DependencyOptions.AutoResolve);
        dependencies.Register<MultiplayerCommandController>(DependencyOptions.AutoResolve);
        dependencies.Register<MultiplayerJoinRequestController>(DependencyOptions.AutoResolve);
        dependencies.Register<PlayersManagementController>(DependencyOptions.AutoResolve);
    }

}
