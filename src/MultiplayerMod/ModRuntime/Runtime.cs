using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer;

namespace MultiplayerMod.ModRuntime;

[UsedImplicitly]
public class Runtime {

    public static Runtime Instance { get; private set; } = null!;

    public DependencyContainer Dependencies { get; }
    public ExecutionContext ExecutionContext => executionContextManager.Context;
    public EventDispatcher EventDispatcher => Dependencies.Get<EventDispatcher>();
    public MultiplayerGame Multiplayer => Dependencies.Get<MultiplayerGame>();

    private readonly ExecutionContextManager executionContextManager = new();

    public Runtime() {
        Dependencies = new DependencyContainer();
        Dependencies.Register(executionContextManager);
        Dependencies.Register<ExecutionLevelManager>();
        Dependencies.Register<EventDispatcher>();
        Dependencies.Register(this);
        Instance = this;
    }

}
