using System;
using System.Collections;
using System.Collections.Generic;
using MultiplayerMod.Core.Collections;

namespace MultiplayerMod.Multiplayer.Objects;

public class MultiplayerObjects : IEnumerable<KeyValuePair<MultiplayerId, object>> {

    private IndexedObjectExtensionMap<object, MultiplayerId> extensions = new();

    public MultiplayerId Register(MultiplayerInstance instance) {
        instance.Id ??= new MultiplayerId(Guid.NewGuid());
        extensions[instance.Id] = instance.gameObject;
        return instance.Id;
    }

    public MultiplayerId Register(object instance) {
        var id = new MultiplayerId(Guid.NewGuid());
        extensions[id] = instance;
        return id;
    }

    public void Remove(MultiplayerId id) => extensions.Remove(id);

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

    public void Synchronize() {
        extensions = new IndexedObjectExtensionMap<object, MultiplayerId>();
        SynchronizeWithTracker();
        // TODO: Sync chores
    }

    public void SynchronizeWithTracker() {
        var kPrefabIds = KPrefabIDTracker.Get().prefabIdMap.Values;
        foreach (var kPrefabId in kPrefabIds) {
            if (kPrefabId == null)
                return;
            var gameObject = kPrefabId.gameObject;
            var instance = gameObject.GetComponent<MultiplayerInstance>();
            instance.Id = new MultiplayerId(kPrefabId.InstanceID);
            Register(instance);
        }
    }

    public IEnumerator<KeyValuePair<MultiplayerId, object>> GetEnumerator() => extensions.GetEnumeratorByValue();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}
