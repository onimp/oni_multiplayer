using System.Collections.Generic;
using MultiplayerMod.Core.Collections;

namespace MultiplayerMod.Multiplayer.Objects;

public class MultiplayerObjectsIndex {

    private Dictionary<object, MultiplayerObject> objects = new(new IdentityEqualityComparer<object>());
    private Dictionary<MultiplayerId, object> instances = new();

    public object this[MultiplayerObject key] {
        set {
            objects[value] = key;
            instances[key.Id] = value;
        }
    }

    public object this[MultiplayerId key] => instances[key];

    public bool Remove(object instance) {
        if (!objects.TryGetValue(instance, out var @object))
            return false;

        objects.Remove(instance);
        instances.Remove(@object.Id);
        return true;
    }

    public bool Remove(MultiplayerId id) {
        if (!instances.TryGetValue(id, out var instance))
            return false;

        objects.Remove(instance);
        instances.Remove(id);
        return true;
    }

    public bool TryGetObject(object instance, out MultiplayerObject? @object) =>
        objects.TryGetValue(instance, out @object);

    public bool TryGetInstance(MultiplayerId id, out object? instance) =>
        instances.TryGetValue(id, out instance);

    public IEnumerable<KeyValuePair<object, MultiplayerObject>> GetEnumerable() => objects;

    public void Clear(bool force = true) {
        if (force) {
            ClearIndexes();
            return;
        }

        var survivedObjects = new Dictionary<object, MultiplayerObject>(new IdentityEqualityComparer<object>());
        var survivedInstances = new Dictionary<MultiplayerId, object>();
        foreach (var (instance, @object) in objects) {
            if (!@object.Persistent)
                continue;

            survivedObjects[instance] = @object;
            survivedInstances[@object.Id] = instance;
        }
        ClearIndexes();
        objects = survivedObjects;
        instances = survivedInstances;
    }

    private void ClearIndexes() {
        objects.Clear();
        instances.Clear();
    }

}
