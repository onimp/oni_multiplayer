using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime.Context;

namespace MultiplayerMod.ModRuntime;

[UsedImplicitly]
public class Runtime {

    public static Runtime Instance { get; private set; } = null!;

    public DependencyContainer Dependencies { get; }
    public ExecutionContext ExecutionContext => executionContextManager.Context;

    private readonly ExecutionContextManager executionContextManager = new();

    public Runtime() {
        Dependencies = new DependencyContainer();
        Dependencies.Register(executionContextManager);
        Dependencies.Register<ExecutionLevelManager>();
        Instance = this;
    }

}
