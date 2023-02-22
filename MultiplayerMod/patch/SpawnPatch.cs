using HarmonyLib;
using MultiplayerMod.multiplayer;
using MultiplayerMod.multiplayer.effect;
using MultiplayerMod.steam;
using UnityEngine;

namespace MultiplayerMod.patch
{

    [HarmonyPatch(typeof(WorldGenSpawner), "OnSpawn")]
    public static class SpawnPatch
    {
        public static void Postfix()
        {
            if (MultiplayerState.MultiplayerRole == MultiplayerState.Role.None) return;

            var go = new GameObject();
            go.AddComponent<PlayerStateEffect>();
            go.AddComponent<WorldDebugDiffer>();

            MultiplayerState.WorldLoaded();
            if (MultiplayerState.MultiplayerRole != MultiplayerState.Role.Server) return;

            var multiplayerGameObject = new GameObject();
            multiplayerGameObject.AddComponent<Server>();
            multiplayerGameObject.AddComponent<ServerActions>();
        }
    }

}
