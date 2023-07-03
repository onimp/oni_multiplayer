using System.Collections.Generic;
using System.Linq;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Logging;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects;

public class MultiplayerObjects {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<MultiplayerObjects>();

    private readonly MultiplayerIdentityProvider provider = Container.Get<MultiplayerIdentityProvider>();

    private Dictionary<MultiplayerId, GameObject> objects = new();

    public void Add(MultiplayerInstance instance) {
        instance.Id ??= provider.GetNextId();
        objects[instance.Id] = instance.gameObject;
    }

    public void Remove(MultiplayerId id) => objects.Remove(id);

    public GameObject this[MultiplayerId id] {
        get {
            if (!objects.TryGetValue(id, out var result))
                log.Warning($"Object {id} not found");
            return result;
        }
    }

    public void Clear() => objects = new Dictionary<MultiplayerId, GameObject>();

    public void Rebuild() {
        objects = KPrefabIDTracker.Get().prefabIdMap.ToDictionary(
            entry => new MultiplayerId(null, entry.Key),
            entry => entry.Value.gameObject
        );
    }

}
