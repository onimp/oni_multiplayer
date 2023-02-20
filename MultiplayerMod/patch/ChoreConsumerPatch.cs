using System.Collections.Generic;
using System.Linq;
using MultiplayerMod.multiplayer;
using UnityEngine;

namespace MultiplayerMod.patch
{

    // [HarmonyPatch(typeof(ChoreConsumer), nameof(ChoreConsumer.FindNextChore))]
    public static class ChoreConsumerPatch
    {
        private static readonly Dictionary<int, Chore.Precondition.Context?> ServerInstanceChoreDictionary = new();

        public static void Postfix(
            ChoreConsumer __instance,
            ref Chore.Precondition.Context out_context,
            ref bool __result
        )
        {
            if (MultiplayerState.MultiplayerRole != MultiplayerState.Role.Client) return;

            var instanceId = __instance.gameObject.GetComponent<KPrefabID>().InstanceID;
            if (!ServerInstanceChoreDictionary.ContainsKey(instanceId))
            {
                ServerInstanceChoreDictionary[instanceId] = out_context;
                return;
            }

            var choreContext = ServerInstanceChoreDictionary[instanceId];
            __result = choreContext != null;
            out_context = choreContext ?? out_context;

            ServerInstanceChoreDictionary[instanceId] = null;
        }

        public static void AddServerChore(object[] payload)
        {
            var instanceId = (int)payload[0];
            var choreId = (int)payload[1];
            var serverChoreName = (string)payload[2];
            var serverChoreNames = (string)payload[3];

            var prefabID = Object.FindObjectsOfType<KPrefabID>().FirstOrDefault(a => a.InstanceID == instanceId);
            if (prefabID == null)
            {
                Debug.LogWarning($"Multiplayer: KPrefabID not found {instanceId}");
                return;
            }

            var clientChoreNames = ChoreDriverPatch.GetAllChoreNames(prefabID);

            if (serverChoreNames != clientChoreNames)
            {
                Debug.LogWarning("Client chores != Server Chores");
                Debug.LogWarning(clientChoreNames);
                Debug.LogWarning(serverChoreNames);
            }

            var consumer = prefabID.GetComponent<ChoreConsumer>();
            if (consumer == null)
            {
                Debug.LogWarning($"Multiplayer: consumer not found {prefabID}");
                return;
            }

            ServerInstanceChoreDictionary[instanceId] = FindContext(consumer, choreId, serverChoreName);
        }

        private static Chore.Precondition.Context? FindContext(
            ChoreConsumer __instance,
            int choreId,
            string serverChoreType
        )
        {
            var chores = __instance.GetProviders()
                .SelectMany(provider => provider.choreWorldMap.Values.SelectMany(x => x)).ToArray();

            foreach (var chore in chores.Where(chore => chore.id == choreId))
            {
                if (chore.GetType().ToString() != serverChoreType)
                {
                    Debug.LogWarning(
                        $"Chore type is not equal client: {chore.GetType()} != server {serverChoreType}"
                    );
                    continue;
                }

                return new Chore.Precondition.Context(chore, __instance.consumerState, true);
            }

            var choreByType = chores.Where(chore => chore.GetType().ToString() == serverChoreType).ToArray();
            if (choreByType.Count() == 1)
            {
                Debug.Log("Not found chore by id but found by type");
                return new Chore.Precondition.Context(choreByType.First(), __instance.consumerState, true);
            }

            Debug.LogWarning($"Multiplayer: Chore not found {choreId} {serverChoreType}");
            return null;
        }
    }

}
