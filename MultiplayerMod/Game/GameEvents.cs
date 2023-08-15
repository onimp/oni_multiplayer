using System;
using HarmonyLib;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Scheduling;
using UnityEngine;

namespace MultiplayerMod.Game;

[HarmonyPatch]
public static class GameEvents {

    public static event System.Action? GameStarted;
    public static event Action<GameObject>? GameObjectCreated;

    // ReSharper disable once UnusedMember.Local
    [HarmonyPostfix]
    [HarmonyPatch(typeof(global::Game), nameof(global::Game.SetGameStarted))]
    private static void SetGameStarted() => Dependencies.Get<UnityTaskScheduler>()
        .Run(() => { GameStarted?.Invoke(); });

    // ReSharper disable once UnusedMember.Local
    [HarmonyPostfix]
    [HarmonyPatch(
        typeof(SaveLoadRoot),
        nameof(SaveLoadRoot.Load),
        typeof(GameObject),
        typeof(Vector3),
        typeof(Quaternion),
        typeof(Vector3),
        typeof(IReader)
    )]
    private static void SaveLoad(SaveLoadRoot __result) {
        if (__result != null)
            GameObjectCreated?.Invoke(__result.gameObject);
    }

    // ReSharper disable once UnusedMember.Local, InconsistentNaming
    [HarmonyPostfix]
    [HarmonyPatch(
        typeof(Util),
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

        GameObjectCreated?.Invoke(__result);
    }

}
