using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Game.Objects;

namespace MultiplayerMod.Game.Extension;

public class GameExtensionsConfigurator {

    public GameExtensionsConfigurator() {
        KInstantiateEvents.Create += gameObject => gameObject.AddComponent<GameObjectExtension>();
        GameEvents.GameStarted += () => KPrefabIDTracker.Get().prefabIdMap.Values
            .ForEach(it => it.gameObject.AddComponent<GameObjectExtension>());
    }

}
