using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Multiplayer.Components;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
using MultiplayerMod.Multiplayer.World.Debug;

namespace MultiplayerMod.Multiplayer.CoreOperations;

[Dependency, UsedImplicitly]
public class MultiplayerGameObjectsSpawner {

    public MultiplayerGameObjectsSpawner(EventDispatcher events) {
        events.Subscribe<GameStartedEvent>(OnGameStarted);
    }

    private void OnGameStarted(GameStartedEvent _) {
        UnityObject.CreateWithComponent<DrawCursorComponent, WorldDebugSnapshotRunner>();
    }

}
