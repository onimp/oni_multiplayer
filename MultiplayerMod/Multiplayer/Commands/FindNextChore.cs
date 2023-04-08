using System;
using System.Linq;
using JetBrains.Annotations;
using MultiplayerMod.Game.Chores;
using Object = UnityEngine.Object;

namespace MultiplayerMod.Multiplayer.Commands;

[Serializable]
public class FindNextChore : IMultiplayerCommand {

    private static Core.Logging.Logger log = new(typeof(FindNextChore));

    private string instanceType;
    private int instanceId;
    private int choreId;
    private string choreName;
    private int choreCell;

    public FindNextChore(string instanceType, int instanceId, int choreId, string choreName, int choreCell) {
        this.instanceType = instanceType;
        this.instanceId = instanceId;
        this.choreId = choreId;
        this.choreName = choreName;
        this.choreCell = choreCell;
    }

    public void Execute() {
        var prefabID = Object.FindObjectsOfType<KPrefabID>().FirstOrDefault(a => a.InstanceID == instanceId);
        if (prefabID == null) {
            log.Warning($"Multiplayer: KPrefabID not found {instanceId}");
            return;
        }
        var consumer = prefabID.GetComponent<ChoreConsumer>();
        if (consumer == null) {
            log.Warning(
                $"Multiplayer: Consumer does not exists at KPrefabId with desired ID {instanceId}. Id collision??"
            );
            return;
        }
        if (instanceType != consumer.ToString()) {
            log.Warning(
                $"Multiplayer: Consumer type {consumer} is not equal to the server {instanceType}."
            );
            return;
        }

        var choreContext = FindContext(consumer, choreId, choreCell, choreName);

        HostChores.Index[instanceId] = choreContext;
    }

    private static Chore.Precondition.Context? FindContext(
        ChoreConsumer instance,
        int choreId,
        int cell,
        string serverChoreType
    ) {
        var chores = instance.GetProviders()
            .SelectMany(provider => provider.choreWorldMap.Values.SelectMany(x => x))
            .ToArray();

        Chore choreWithIdCollision = null;
        var chore = FindFullMatch(
            chores,
            choreId,
            cell,
            serverChoreType,
            ref choreWithIdCollision
        ) ?? FindMatchWithoutId(chores, cell, serverChoreType);

        if (chore == null) {
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
            log.Warning($"Multiplayer: Chore global search. Result is {chore != null}");
        }

        if (choreWithIdCollision != null)
            choreWithIdCollision.id = new Random().Next();

        if (chore == null) {
            log.Warning($"Multiplayer: Chore not found {choreId} {serverChoreType}");
            return null;
        }

        if (chore.id != choreId) {
            chore.id = choreId;
            log.Info("Multiplayer: Corrected chore id.");
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
    ) {
        var result = chores.FirstOrDefault(
            chore => chore.id == choreId
        );

        var clientChoreCell = Grid.PosToCell(result.gameObject.transform.GetPosition());
        if (choreCell != clientChoreCell) {
            log.Warning(
                $"Multiplayer: Server chore pos != client chore pos. {choreId}. {choreCell} != {clientChoreCell}"
            );
            choreWithIdCollision = result;
            return null;
        }
        if (result.GetType().ToString() != choreType) {
            log.Warning(
                $"Chore type is not equal client: {result.GetType()} != server {choreType}"
            );
            choreWithIdCollision = result;
            return null;
        }
        return result;
    }

    private static Chore FindMatchWithoutId(Chore[] chores, int choreCell, string choreType) {
        return chores.FirstOrDefault(
            chore => chore.GetType().ToString() == choreType &&
                     choreCell == Grid.PosToCell(chore.gameObject.transform.GetPosition())
        );
    }

}
