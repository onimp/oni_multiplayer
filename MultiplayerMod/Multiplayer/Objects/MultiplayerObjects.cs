using System.Collections.Generic;
using MultiplayerMod.Core.Dependency;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects;

public class MultiplayerObjects {

    private readonly MultiplayerIdentityProvider provider = Container.Get<MultiplayerIdentityProvider>();

    private readonly Dictionary<MultiplayerId, GameObject> objects = new();

    public void Add(MultiplayerInstance instance) {
        instance.Id ??= provider.GetNextId();
        objects[instance.Id] = instance.gameObject;
    }

    public void Remove(MultiplayerId id) => objects.Remove(id);

    public GameObject this[MultiplayerId id] => objects[id];

    public void Clear() => objects.Clear();

}
