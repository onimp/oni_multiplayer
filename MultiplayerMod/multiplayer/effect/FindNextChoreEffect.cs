using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MultiplayerMod.multiplayer.effect
{

    public static class FindNextChoreEffect
    {
        public static Dictionary<int, Chore.Precondition.Context?> ServerInstanceChoreDictionary { get; } = new();

        public static void AddServerChore(object[] payload)
        {
            var instanceId = (int)payload[0];
            var choreId = (int)payload[1];
            var serverChoreName = (string)payload[2];
            var serverChoreCell = (int)payload[3];

            var prefabID = Object.FindObjectsOfType<KPrefabID>().FirstOrDefault(a => a.InstanceID == instanceId);
            if (prefabID == null)
            {
                Debug.LogWarning($"Multiplayer: KPrefabID not found {instanceId}");
                return;
            }
            var consumer = prefabID.GetComponent<ChoreConsumer>();
            if (consumer == null)
            {
                Debug.LogWarning(
                    $"Multiplayer: Consumer does not exists at KPrefabId with desired ID {instanceId}. Id collision??"
                );
                return;
            }

            var choreContext = FindContext(consumer, choreId, serverChoreName);
            if (choreContext != null)
            {
                var clientChoreCell = Grid.PosToCell(choreContext.Value.chore.gameObject.transform.GetPosition());
                if (serverChoreCell != clientChoreCell)
                {
                    Debug.LogWarning(
                        $"Multiplayer: Server chore pos != client chore pos. {instanceId}. {serverChoreCell} != {clientChoreCell}"
                    );
                    return;
                }
            }

            ServerInstanceChoreDictionary[instanceId] = choreContext;
        }

        private static Chore.Precondition.Context? FindContext(
            ChoreConsumer instance,
            int choreId,
            string serverChoreType
        )
        {
            var chores = instance.GetProviders()
                .SelectMany(provider => provider.choreWorldMap.Values.SelectMany(x => x)).ToArray();

            foreach (var chore in chores.Where(chore => chore.id == choreId))
            {
                if (chore.GetType().ToString() == serverChoreType)
                    return new Chore.Precondition.Context(chore, instance.consumerState, true);

                Debug.LogWarning(
                    $"Chore type is not equal client: {chore.GetType()} != server {serverChoreType}"
                );
            }

            var choreByType = chores.Where(chore => chore.GetType().ToString() == serverChoreType).ToArray();
            if (choreByType.Count() == 1)
            {
                Debug.Log("Not found chore by id but found by type");
                return new Chore.Precondition.Context(choreByType.First(), instance.consumerState, true);
            }

            Debug.LogWarning($"Multiplayer: Chore not found {choreId} {serverChoreType}");
            return null;
        }

    }

}
