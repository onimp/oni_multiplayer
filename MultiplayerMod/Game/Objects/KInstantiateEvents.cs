using System;
using HarmonyLib;
using UnityEngine;

namespace MultiplayerMod.Game.Objects;

[HarmonyPatch(typeof(Util))]
public static class KInstantiateEvents {

    public static event Action<GameObject>? Create;

    // ReSharper disable once UnusedMember.Local, InconsistentNaming
    [HarmonyPostfix]
    [HarmonyPatch(
        nameof(Util.KInstantiate),
        typeof(GameObject),
        typeof(Vector3),
        typeof(Quaternion),
        typeof(GameObject),
        typeof(string),
        typeof(bool),
        typeof(int)
    )]
    private static void InstantiatePostfix(GameObject __result, bool initialize_id) {
        if (!initialize_id)
            return;
        var kPrefabId = __result.GetComponent<KPrefabID>();
        if (kPrefabId == null)
            return;
        Create?.Invoke(__result);
    }

}
