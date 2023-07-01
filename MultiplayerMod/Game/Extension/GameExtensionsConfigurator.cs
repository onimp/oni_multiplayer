using MultiplayerMod.Game.Objects;

namespace MultiplayerMod.Game.Extension;

public class GameExtensionsConfigurator {

    public GameExtensionsConfigurator() {
        KInstantiateEvents.Create += gameObject => gameObject.AddComponent<GameObjectExtension>();
        KPrefabIdEvents.Deserialize += kPrefabId => kPrefabId.gameObject.AddComponent<GameObjectExtension>();
    }

}
