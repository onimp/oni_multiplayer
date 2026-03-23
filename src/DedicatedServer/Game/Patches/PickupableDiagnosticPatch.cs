using System;
using HarmonyLib;

namespace DedicatedServer.Game.Patches;

/// <summary>
/// Diagnostic patch: intercepts Pickupable.OnPrefabInit and Pickupable.OnSpawn exceptions
/// BEFORE KMonoBehaviour's internal try/catch swallows them, logging the full stack trace.
///
/// Background: KMonoBehaviour.InitializeComponent() and Spawn() each have a try/catch that
/// catches ALL exceptions and logs only a truncated one-liner via DebugUtil.LogExceptionCallstack.
/// The Debug.LogError patch in MiscPatches re-routes those to Console.Error, but the message
/// is buried in ~~~!...!~~~ markers and may be hard to parse in bulk logs.
///
/// This Finalizer fires AFTER the original method (including when it throws), giving us a
/// clearly-labelled stack trace. It returns the original exception so KMonoBehaviour's own
/// catch still handles it normally — behaviour is unchanged, only visibility improves.
///
/// USAGE: Run dedicated server, look for [DIAG-Pickupable] lines in stderr output. The first
/// such line reveals the exact NPE site (e.g. "at Workable.OnPrefabInit() [0x000XX]").
/// Remove this file after root cause is confirmed.
/// </summary>
[HarmonyPatch(typeof(Pickupable))]
public static class PickupableDiagnosticPatch {

    private static int _prefabInitErrors;
    private static int _spawnErrors;
    private const int MaxLoggedErrors = 3; // cap at 3 to avoid log spam

    [HarmonyFinalizer]
    [HarmonyPatch("OnPrefabInit")]
    static Exception OnPrefabInitFinalizer(Exception __exception, Pickupable __instance) {
        if (__exception != null && _prefabInitErrors < MaxLoggedErrors) {
            _prefabInitErrors++;
            Console.Error.WriteLine(
                $"[DIAG-Pickupable.OnPrefabInit #{_prefabInitErrors}] " +
                $"go={__instance?.name ?? "NULL"} : {__exception}");
        }
        return __exception; // rethrow — let KMonoBehaviour's catch handle it
    }

    [HarmonyFinalizer]
    [HarmonyPatch("OnSpawn")]
    static Exception OnSpawnFinalizer(Exception __exception, Pickupable __instance) {
        if (__exception != null && _spawnErrors < MaxLoggedErrors) {
            _spawnErrors++;
            Console.Error.WriteLine(
                $"[DIAG-Pickupable.OnSpawn #{_spawnErrors}] " +
                $"go={__instance?.name ?? "NULL"} : {__exception}");
        }
        return __exception; // rethrow
    }
}
