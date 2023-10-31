using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Multiplayer.Components;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
using MultiplayerMod.Multiplayer.World.Debug;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.CoreOperations;

[Dependency, UsedImplicitly]
public class MultiplayerGameObjectsSpawner {

    public MultiplayerGameObjectsSpawner(EventDispatcher events) {
        events.Subscribe<GameStartedEvent>(OnGameStarted);
    }

    // ReSharper disable once ObjectCreationAsStatement
    private void OnGameStarted(GameStartedEvent _) {
        var components = new[] {
#if DEBUG
            typeof(WorldDebugSnapshotRunner),
#endif
            typeof(CursorManager),
            typeof(MultiplayerPlayerNotifier)
        };
        new GameObject("Multiplayer", components);
    }

}
