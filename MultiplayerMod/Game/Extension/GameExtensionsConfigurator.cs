namespace MultiplayerMod.Game.Extension;

public class GameExtensionsConfigurator {

    public GameExtensionsConfigurator() {
        GameEvents.GameObjectCreated += it => it.AddComponent<GameObjectExtension>();
    }

}
