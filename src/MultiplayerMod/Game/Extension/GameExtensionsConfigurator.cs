using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;

namespace MultiplayerMod.Game.Extension;

[Dependency, UsedImplicitly]
public class GameExtensionsConfigurator {

    public GameExtensionsConfigurator() {
        GameEvents.GameObjectCreated += it => it.AddComponent<GameObjectExtension>();
    }

}
