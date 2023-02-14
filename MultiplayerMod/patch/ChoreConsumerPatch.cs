using System.Collections.Generic;
using HarmonyLib;
using MultiplayerMod.multiplayer;

namespace MultiplayerMod.patch
{
    [HarmonyPatch(typeof(ChoreConsumer), nameof(ChoreConsumer.FindNextChore))]
    public static class ChoreConsumerPatch
    {
        private static readonly Dictionary<int, Chore.Precondition.Context> ServerChores = new();

        public static void Postfix(ChoreConsumer __instance, ref Chore.Precondition.Context out_context,
            ref bool __result)
        {
            if (MultiplayerState.MultiplayerRole != MultiplayerState.Role.Client) return;
            
            __result = false;
            var instanceId = __instance.gameObject.GetComponent<KPrefabID>().InstanceID;
            if (!ServerChores.ContainsKey(instanceId)) return;
            __result = true;
            out_context = ServerChores[instanceId];
            ServerChores.Remove(instanceId);
        }

        public static void AddServerChore(int instanceId, Chore.Precondition.Context context)
        {
            ServerChores[instanceId] = context;
        }
    }
}