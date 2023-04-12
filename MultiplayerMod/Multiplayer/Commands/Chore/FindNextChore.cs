using System;
using System.Linq;
using JetBrains.Annotations;
using MultiplayerMod.Game.Chores;
using Object = UnityEngine.Object;

namespace MultiplayerMod.Multiplayer.Commands.Chore;

[Serializable]
public class FindNextChore : IMultiplayerCommand
{

    private static Core.Logging.Logger log = new(typeof(FindNextChore));

    private string instanceType;
    private int instanceId;
    private int choreId;
    private string choreType;
    private int choreCell;

    public FindNextChore(FindNextChoreEventArgs findNextChoreEventArgs)
    {
        instanceType = findNextChoreEventArgs.InstantType.ToString();
        instanceId = findNextChoreEventArgs.InstanceId;
        choreId = findNextChoreEventArgs.ChoreId;
        choreType = findNextChoreEventArgs.ChoreType.ToString();
        choreCell = findNextChoreEventArgs.ChoreCell;
    }

    public void Execute()
    {
        var prefabID = Object.FindObjectsOfType<KPrefabID>().FirstOrDefault(a => a.InstanceID == instanceId);
        if (prefabID == null)
        {
            log.Warning($"Multiplayer: KPrefabID not found {instanceId}");
            return;
        }
        var consumer = prefabID.GetComponent<ChoreConsumer>();
        if (consumer == null)
        {
            log.Warning(
                $"Multiplayer: Consumer does not exists at KPrefabId with desired ID {instanceId}. Id collision??"
            );
            return;
        }
        if (instanceType != consumer.GetType().ToString())
        {
            log.Warning(
                $"Multiplayer: Consumer type {consumer.GetType()} is not equal to the server {instanceType}."
            );
            return;
        }

        var choreContext = FindContext(consumer, choreId, choreCell, choreType);

        HostChores.Index[instanceId] = choreContext;
    }

    private static global::Chore.Precondition.Context? FindContext(
        ChoreConsumer instance,
        int choreId,
        int cell,
        string serverChoreType
    )
    {
        global::Chore choreWithIdCollision = null;
        var chore = FindInInstance(
                        instance,
                        choreId,
                        cell,
                        serverChoreType,
                        ref choreWithIdCollision
                    )
                    ?? FindInGlobal(
                        instance,
                        choreId,
                        cell,
                        serverChoreType,
                        ref choreWithIdCollision
                    );

        if (choreWithIdCollision != null)
        {
            choreWithIdCollision.id = new Random().Next();
            choreWithIdCollision.driver = null;
        }

        if (chore == null)
        {
            log.Warning($"Multiplayer: Chore not found {choreId} {serverChoreType}");
            return null;
        }

        if (chore.id != choreId)
        {
            chore.id = choreId;
            log.Info("Multiplayer: Corrected chore id.");
        }

        chore.driver = null;
        return new global::Chore.Precondition.Context(chore, instance.consumerState, true);
    }

    private static global::Chore FindInInstance(
        ChoreConsumer instance,
        int choreId,
        int cell,
        string serverChoreType,
        ref global::Chore choreWithIdCollision
    )
    {
        var chores = instance.GetProviders()
            .SelectMany(provider => provider.choreWorldMap.Values.SelectMany(x => x))
            .ToArray();
        return FindFullMatch(
            chores,
            choreId,
            cell,
            serverChoreType,
            ref choreWithIdCollision
        ) ?? FindMatchWithoutId(chores, cell, serverChoreType);
    }

    private static global::Chore FindInGlobal(
        ChoreConsumer instance,
        int choreId,
        int cell,
        string serverChoreType,
        ref global::Chore choreWithIdCollision
    )
    {
        var globalChores =
            Object.FindObjectsOfType<ChoreConsumer>()
                .SelectMany(
                    consumer => consumer.GetProviders()
                        .SelectMany(provider => provider.choreWorldMap.Values.SelectMany(x => x)).ToArray()
                ).ToArray();

        var chore = FindFullMatch(
            globalChores,
            choreId,
            cell,
            serverChoreType,
            ref choreWithIdCollision
        ) ?? FindMatchWithoutId(globalChores, cell, serverChoreType);
        log.Info(
            $"Multiplayer: Chore global search. Result is {chore != null}. {instance.GetType()} {serverChoreType}"
        );
        return chore;
    }

    [CanBeNull]
    private static global::Chore FindFullMatch(
        global::Chore[] chores,
        int choreId,
        int choreCell,
        string choreType,
        ref global::Chore choreWithIdCollision
    )
    {
        var result = chores.FirstOrDefault(
            chore => chore.id == choreId
        );

        if (result == null) return null;

        var clientChoreCell = Grid.PosToCell(result.gameObject.transform.GetPosition());
        if (choreCell != clientChoreCell)
        {
            log.Warning(
                $"Multiplayer: Server chore pos != client chore pos. {choreId}. {choreCell} != {clientChoreCell}"
            );
            choreWithIdCollision = result;
            return null;
        }
        if (result.GetType().ToString() != choreType)
        {
            log.Warning(
                $"Chore type is not equal client: {result.GetType()} != server {choreType}"
            );
            choreWithIdCollision = result;
            return null;
        }
        return result;
    }

    private static global::Chore FindMatchWithoutId(global::Chore[] chores, int choreCell, string choreType)
    {
        return chores.FirstOrDefault(
            chore => chore.GetType().ToString() == choreType &&
                     choreCell == Grid.PosToCell(chore.gameObject.transform.GetPosition())
        );
    }

}
