using System;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

[Serializable]
public class StaticKPrefabIdReference : GameObjectReference {

    public int KPrefabId { get; }

    public StaticKPrefabIdReference(GameObject gameObject) {
        KPrefabId = gameObject.GetComponent<KPrefabID>().InstanceID;
    }

    protected override GameObject? Resolve() {
        KPrefabIDTracker.Get().prefabIdMap.TryGetValue(KPrefabId, out var component);
        return component?.gameObject;
    }

}
