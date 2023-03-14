using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Random = System.Random;

namespace MultiplayerMod.Multiplayer.Effect
{

    public static class FindNextChoreEffect
    {
        public static Dictionary<int, Chore.Precondition.Context?> ServerInstanceChoreDictionary { get; } = new();

        public static void AddServerChore(object[] payload)
        {
            var instanceType = (string)payload[0];
            var instanceId = (int)payload[1];
            var choreId = (int)payload[2];
            var choreName = (string)payload[3];
            var choreCell = (int)payload[4];

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
            if (instanceType != consumer.ToString())
            {
                Debug.LogWarning(
                    $"Multiplayer: Consumer type {consumer} is not equal to the server {instanceType}."
                );
                return;
            }

            var choreContext = FindContext(consumer, choreId, choreCell, choreName);

            ServerInstanceChoreDictionary[instanceId] = choreContext;
        }

        private static Chore.Precondition.Context? FindContext(
            ChoreConsumer instance,
            int choreId,
            int cell,
            string serverChoreType
        )
        {
            var chores = instance.GetProviders()
                .SelectMany(provider => provider.choreWorldMap.Values.SelectMany(x => x)).ToArray();

            Chore choreWithIdCollision = null;
            var chore = FindFullMatch(
                chores,
                choreId,
                cell,
                serverChoreType,
                ref choreWithIdCollision
            ) ?? FindMatchWithoutId(chores, cell, serverChoreType);

            if (chore == null)
            {
                var globalChores =
                    Object.FindObjectsOfType<ChoreConsumer>()
                        .SelectMany(
                            consumer => consumer.GetProviders()
                                .SelectMany(provider => provider.choreWorldMap.Values.SelectMany(x => x)).ToArray()
                        ).ToArray();

                chore = FindFullMatch(
                    globalChores,
                    choreId,
                    cell,
                    serverChoreType,
                    ref choreWithIdCollision
                ) ?? FindMatchWithoutId(globalChores, cell, serverChoreType);
                Debug.LogWarning($"Multiplayer: Chore global search. Result is {chore != null}");
            }

            choreWithIdCollision?.SetProperty<Chore>("id", new Random().Next());

            if (chore == null)
            {
                Debug.LogWarning($"Multiplayer: Chore not found {choreId} {serverChoreType}");
                return null;
            }

            if (chore.id != choreId)
            {
                chore.SetProperty<Chore>("id", choreId);
                Debug.Log("Multiplayer: Corrected chore id.");
            }

            return new Chore.Precondition.Context(chore, instance.consumerState, true);
        }

        [CanBeNull]
        private static Chore FindFullMatch(
            Chore[] chores,
            int choreId,
            int choreCell,
            string choreType,
            ref Chore choreWithIdCollision
        )
        {
            var result = chores.FirstOrDefault(
                chore => chore.id == choreId
            );

            var clientChoreCell = Grid.PosToCell(result.gameObject.transform.GetPosition());
            if (choreCell != clientChoreCell)
            {
                Debug.LogWarning(
                    $"Multiplayer: Server chore pos != client chore pos. {choreId}. {choreCell} != {clientChoreCell}"
                );
                choreWithIdCollision = result;
                return null;
            }
            if (result.GetType().ToString() != choreType)
            {
                Debug.LogWarning(
                    $"Chore type is not equal client: {result.GetType()} != server {choreType}"
                );
                choreWithIdCollision = result;
                return null;
            }
            return result;
        }

        private static Chore FindMatchWithoutId(Chore[] chores, int choreCell, string choreType)
        {
            return chores.FirstOrDefault(
                chore => chore.GetType().ToString() == choreType &&
                         choreCell == Grid.PosToCell(chore.gameObject.transform.GetPosition())
            );
        }
    }

}
