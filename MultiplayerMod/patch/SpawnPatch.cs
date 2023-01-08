using HarmonyLib;
using MultiplayerMod.steam;

namespace MultiplayerMod.patch
{
    [HarmonyPatch(typeof(WorldGenSpawner), "OnSpawn")]
    public class SpawnPatch
    {
        public static void Postfix()
        {
            Debug.Log("World is spawned. Starting server if needed.");
            Game.Instance.GetComponent<Server>().HostServerIfNeeded();
        }
    }
}