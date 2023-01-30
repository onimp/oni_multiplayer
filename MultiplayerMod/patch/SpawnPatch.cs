using System.Linq;
using HarmonyLib;
using MultiplayerMod.multiplayer;
using MultiplayerMod.steam;
using UnityEngine;

namespace MultiplayerMod.patch
{
    [HarmonyPatch(typeof(WorldGenSpawner), "OnSpawn")]
    public static class SpawnPatch
    {
        public static bool HostServerAfterStart { get; set; }
        public static void Postfix()
        {
            Object.FindObjectsOfType<ClientActions>().FirstOrDefault()!.WorldSpawned = true;
            if (!HostServerAfterStart) return;
            HostServerAfterStart = false;
            var multiplayerGameObject = new GameObject();
            multiplayerGameObject.AddComponent<Server>();
            multiplayerGameObject.AddComponent<ServerActions>();
        }
    }
}