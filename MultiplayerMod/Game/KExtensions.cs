using UnityEngine;

namespace MultiplayerMod.Game;

public static class KExtensions {

    public static int? GetKPrefabInstanceId(this GameObject gameObject) =>
        gameObject.GetComponent<KPrefabID>()?.InstanceID;

}
