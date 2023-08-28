using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Game.Extension;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Loader;

namespace MultiplayerMod.Game;

[UsedImplicitly]
public class GameComponentLoader : IModComponentLoader {

    public void Load(Runtime runtime) {
        runtime.Dependencies.Register<GameExtensionsConfigurator>(DependencyOptions.AutoResolve);
    }

}
