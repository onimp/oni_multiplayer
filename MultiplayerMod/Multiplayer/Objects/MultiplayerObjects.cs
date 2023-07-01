using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects;

public class MultiplayerObjects {

    private readonly Dictionary<long, GameObject> objects = new();

    public void Add(MultiplayerInstance instance) {
        instance.Id ??= MultiplayerIdentityProvider.GetNextId();
        objects[instance.Id.Value] = instance.gameObject;
    }

    public void Remove(long objectId) => objects.Remove(objectId);

    public GameObject this[long id] => objects[id];

    public void Clear() => objects.Clear();

}
