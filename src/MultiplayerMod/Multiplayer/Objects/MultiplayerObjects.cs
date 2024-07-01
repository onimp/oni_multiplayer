using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Game;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
using MultiplayerMod.Multiplayer.World;

namespace MultiplayerMod.Multiplayer.Objects;

[Dependency, UsedImplicitly]
public class MultiplayerObjects {

    private readonly MultiplayerObjectsIndex index = new();
    private int generation;

    public MultiplayerObjects(EventDispatcher events) {
        GameEvents.GameObjectCreated += it => it.AddComponent<MultiplayerInstance>();
        var initializer = new SaveGameObjectsInitializer(this);
        events.Subscribe<WorldLoadingEvent>(_ => Clear());
        events.Subscribe<GameQuitEvent>(_ => Clear());
        events.Subscribe<WorldSyncEvent>(
            _ => {
                Clear(force: false);
                initializer.Initialize();
            }
        );
        events.Subscribe<GameReadyEvent>(_ => initializer.Initialize());
    }

    public bool Valid(MultiplayerObject @object) => @object.Persistent || @object.Generation == generation;

    public MultiplayerObject Register(object instance, MultiplayerId? multiplayerId = null, bool persistent = false) {
        var @object = new MultiplayerObject(multiplayerId ?? new MultiplayerId(Guid.NewGuid()), generation, persistent);
        index[@object] = instance;
        return @object;
    }

    private void Clear(bool force = true) {
        index.Clear(force);
        generation++;
    }

    public void Remove(MultiplayerId id) => index.Remove(id);

    public void RemoveObject(object instance) => index.Remove(instance);

    public T? Get<T>(MultiplayerId id) => !index.TryGetInstance(id, out var instance) ? default : (T) instance!;

    public MultiplayerObject? Get(object instance) => !index.TryGetObject(instance, out var @object) ? null : @object;

    public IEnumerable<KeyValuePair<object, MultiplayerObject>> GetEnumerable() => index.GetEnumerable();

}
