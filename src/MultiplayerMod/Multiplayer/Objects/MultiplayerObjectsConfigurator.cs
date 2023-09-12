using JetBrains.Annotations;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Game;
using MultiplayerMod.Multiplayer.CoreOperations.Events;

namespace MultiplayerMod.Multiplayer.Objects;

[UsedImplicitly]
public class MultiplayerObjectsConfigurator {

    public MultiplayerObjectsConfigurator(MultiplayerGame multiplayer, EventDispatcher events) {
        GameEvents.GameObjectCreated += it => it.AddComponent<MultiplayerInstance>();
        events.Subscribe<GameStartedEvent>(_ => multiplayer.Objects.SynchronizeWithTracker());
    }

}
