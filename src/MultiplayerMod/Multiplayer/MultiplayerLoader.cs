using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Loader;
using MultiplayerMod.Multiplayer.Configuration;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Multiplayer.World;

namespace MultiplayerMod.Multiplayer;

[UsedImplicitly]
[ModComponentOrder(ModComponentOrder.Configuration)]
public class MultiplayerLoader : IModComponentLoader {

    public void Load(Runtime runtime) {
        var dependencies = runtime.Dependencies;
        dependencies.Register<WorldManager>();
        dependencies.Register<MultiplayerIdentityProvider>();
        dependencies.Register<MultiplayerGame>();
        dependencies.Register<MultiplayerCommandExecutor>();
        dependencies.Register<MultiplayerObjectsConfigurator>(DependencyOptions.AutoResolve);
        dependencies.Register<PlayerConnectionManager>(DependencyOptions.AutoResolve);
        dependencies.Register<MultiplayerCoordinator>(DependencyOptions.AutoResolve);
    }

}
