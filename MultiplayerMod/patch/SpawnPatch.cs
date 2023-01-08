using HarmonyLib;
using MultiplayerMod.steam;
using UnityEngine;

namespace MultiplayerMod.patch
{
    [HarmonyPatch(typeof(WorldGenSpawner), "OnSpawn")]
    public class SpawnPatch
    {
        public static void Postfix()
        {
            var multiplayerGameObject = new GameObject();
            multiplayerGameObject.AddComponent<Client>();
            multiplayerGameObject.AddComponent<Server>();

            var server = multiplayerGameObject.GetComponent<Server>();
            if (server == null)
            {
                Debug.Log("Server is null");
                return;
            }
            
            Debug.Log("World is spawned. Starting server if needed.");
            server.HostServerIfNeeded();
        }
    }
}