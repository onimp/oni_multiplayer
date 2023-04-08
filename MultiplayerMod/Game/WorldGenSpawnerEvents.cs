using HarmonyLib;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Game;

[HarmonyPatch(typeof(WorldGenSpawner))]
public static class WorldGenSpawnerEvents {

    public static event System.Action Spawned;

    [HarmonyPostfix]
    [HarmonyPatch("OnSpawn")]
    private static void OnSpawn() => PatchControl.RunIfEnabled(() => { Spawned?.Invoke(); });

}
