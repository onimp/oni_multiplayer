using MultiplayerMod.Game;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Multiplayer.Objects;

public class MultiplayerObjectsConfigurator {

    public MultiplayerObjectsConfigurator() {
        GameEvents.GameObjectCreated += it => it.AddComponent<MultiplayerInstance>();
        GameEvents.GameStarted += () => {
            if (MultiplayerGame.Role != MultiplayerRole.None)
                MultiplayerGame.Objects.SynchronizeWithTracker();
        };
    }

}
