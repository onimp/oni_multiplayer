using System;
using HarmonyLib;

namespace DedicatedServer.Game.Patches;

/// <summary>
/// Patch for Navigator — makes navigation headless-safe.
///
/// Root cause of Navigator.OnSpawn NPE:
///   Navigator.OnPrefabInit() calls KInstantiate("TargetLocator") at line 401 BEFORE setting
///   NavGrid at line 406. If that KInstantiate fails (prefab not found → null → GetComponent NPE),
///   OnPrefabInit exits early, NavGrid and PathGrid are never set. Then OnSpawn() calls
///   SetCurrentNavType() → NavGrid.GetNavTypeData() NPE.
///
/// Fix: Prefix on OnSpawn — skip entire navigation setup when NavGrid is null.
/// Dedicated server does not need working pathfinding; dupes must exist without crashing.
/// </summary>
[HarmonyPatch(typeof(Navigator))]
public static class NavigatorPatches {

    [HarmonyPrefix]
    [HarmonyPatch("OnSpawn")]
    static bool OnSpawn(Navigator __instance) {
        if (__instance.NavGrid == null) {
            Console.Error.WriteLine(
                $"[Navigator] OnSpawn skipped: NavGrid null " +
                $"(NavGridName={__instance.NavGridName}, go={__instance.gameObject?.name ?? "?"})");
            return false; // skip OnSpawn entirely — prevents SetCurrentNavType NPE
        }
        return true;
    }
}
