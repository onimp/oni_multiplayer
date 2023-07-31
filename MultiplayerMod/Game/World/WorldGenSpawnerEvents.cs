using HarmonyLib;

namespace MultiplayerMod.Game.World;

[HarmonyPatch(typeof(WorldGenSpawner))]
public static class WorldGenSpawnerEvents {

    public static event System.Action? Spawned;

    [HarmonyPostfix]
    [HarmonyPatch("OnSpawn")]
    // ReSharper disable once UnusedMember.Local
    private static void OnSpawn() => Spawned?.Invoke();

}
