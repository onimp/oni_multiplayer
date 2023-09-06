using JetBrains.Annotations;
using MultiplayerMod.Game;

namespace MultiplayerMod.Multiplayer.Objects;

[UsedImplicitly]
public class MultiplayerObjectsConfigurator {

    public MultiplayerObjectsConfigurator(MultiplayerGame multiplayer) {
        GameEvents.GameObjectCreated += it => it.AddComponent<MultiplayerInstance>();
        GameEvents.GameStarted += () => {
            if (multiplayer.Mode != MultiplayerMode.None)
                multiplayer.Objects.SynchronizeWithTracker();
        };
    }

}
