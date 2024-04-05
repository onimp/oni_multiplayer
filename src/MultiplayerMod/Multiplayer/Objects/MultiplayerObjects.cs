using System;
using System.Collections;
using System.Collections.Generic;
using MultiplayerMod.Core.Collections;

namespace MultiplayerMod.Multiplayer.Objects;

public class MultiplayerObjects : IEnumerable<KeyValuePair<MultiplayerId, object>> {

    private readonly MultiplayerObjectsInitializer initializer;
    private IndexedObjectExtensionMap<object, MultiplayerId> extensions = new();

    public MultiplayerObjects() {
        initializer = new MultiplayerObjectsInitializer(this);
    }

    public MultiplayerId Register(object instance, MultiplayerId? multiplayerId) {
        var id = multiplayerId ?? new MultiplayerId(Guid.NewGuid());
        extensions[id] = instance;
        return id;
    }

    public void Remove(MultiplayerId id) => extensions.Remove(id);

    public T Get<T>(MultiplayerId id) {
        return (T)extensions[id];
    }

    public object? this[MultiplayerId id] {
        get {
            extensions.TryGetKey(id, out var result);
            return result;
        }
        set {
            if (value != null)
                extensions[id] = value;
            else
                extensions.Remove(id);
        }
    }

    public MultiplayerId? this[object instance] {
        get {
            extensions.TryGetValue(instance, out var result);
            return result;
        }
        set {
            if (value != null)
                extensions[instance] = value;
            else
                extensions.Remove(instance);
        }
    }

    public void Reset() {
        extensions = new IndexedObjectExtensionMap<object, MultiplayerId>();
        initializer.Initialize();
    }

    public IEnumerator<KeyValuePair<MultiplayerId, object>> GetEnumerator() => extensions.GetEnumeratorByValue();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}
