using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer.CoreOperations.Binders;

namespace MultiplayerMod.Multiplayer.CoreOperations;

[UsedImplicitly]
public class MultiplayerOperationsConfigurer {

    private readonly DependencyContainer dependencies;

    public MultiplayerOperationsConfigurer(DependencyContainer dependencies) {
        this.dependencies = dependencies;
    }

    public void Configure() {
        dependencies.Resolve<GameEventsBinder>().Bind();
        dependencies.Resolve<HostEventsBinder>().Bind();

        dependencies.Register<GameStateEventsRedirector>(DependencyOptions.AutoResolve);
        dependencies.Register<ExecutionLevelController>(DependencyOptions.AutoResolve);
    }

}
