using UnityEngine;

namespace MultiplayerMod.Game;

public static class KEventSystemExtensions {

    public static void Trigger(this GameObject go, GameHashes hash, object? data = null) {
        var kObject = KObjectManager.Instance.Get(go);
        if (kObject == null || !kObject.hasEventSystem)
            return;

        kObject.GetEventSystem().Trigger(go, (int) hash, data);
    }

}
