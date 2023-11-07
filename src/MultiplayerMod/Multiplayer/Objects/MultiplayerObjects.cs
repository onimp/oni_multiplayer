using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects;

public class MultiplayerObjects : IEnumerable<KeyValuePair<MultiplayerId, GameObject>> {

    private Dictionary<MultiplayerId, GameObject> objects = new();

    public MultiplayerId Register(MultiplayerInstance instance) {
        instance.Id ??= new MultiplayerId(Guid.NewGuid());
        objects[instance.Id] = instance.gameObject;
        return instance.Id;
    }

    public void Remove(MultiplayerId id) => objects.Remove(id);

    public GameObject? this[MultiplayerId id] {
        get {
            objects.TryGetValue(id, out var result);
            return result;
        }
        set {
            if (value != null)
                objects[id] = value;
            else
                objects.Remove(id);
        }
    }

    public void SynchronizeWithTracker() {
        objects = new Dictionary<MultiplayerId, GameObject>();
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

    public IEnumerator<KeyValuePair<MultiplayerId, GameObject>> GetEnumerator() => objects.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}
