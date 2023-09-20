using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;

namespace MultiplayerMod.ModRuntime;

[Dependency, UsedImplicitly]
public class Runtime {

    public static Runtime Instance { get; private set; } = null!;

    public IDependencyContainer Dependencies { get; }

    public Runtime(IDependencyContainer container) {
        Dependencies = container;
        Instance = this;
    }

}
