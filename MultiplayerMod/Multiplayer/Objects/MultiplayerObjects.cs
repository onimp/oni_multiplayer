using System.Collections.Generic;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Logging;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects;

public class MultiplayerObjects {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger(typeof(MultiplayerObjects));

    private readonly MultiplayerIdentityProvider provider = Container.Get<MultiplayerIdentityProvider>();

    private readonly Dictionary<MultiplayerId, GameObject> objects = new();

    public void Add(MultiplayerInstance instance) {
        instance.Id ??= provider.GetNextId();
        objects[instance.Id] = instance.gameObject;
    }

    public void Remove(MultiplayerId id) => objects.Remove(id);

    public GameObject this[MultiplayerId id] {
        get {
            if (!objects.TryGetValue(id, out var result)) {
                log.Warning($"Object {id} not found");
            }
            return result;
        }
    }

    public void Clear() => objects.Clear();

}
