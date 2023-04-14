using System;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.Game.Chores;
using Object = UnityEngine.Object;

namespace MultiplayerMod.Multiplayer.Commands.Chores;

[Serializable]
public class FindNextChore : IMultiplayerCommand {

    private static Core.Logging.Logger log = LoggerFactory.GetLogger<FindNextChore>();

    private string instanceType;
    private int instanceId;
    private int choreId;
    private string choreType;
    private int choreCell;

    public FindNextChore(FindNextChoreEventArgs args) {
        instanceType = args.InstanceType.ToString();
        instanceId = args.InstanceId;
        choreId = args.ChoreId;
        choreType = args.ChoreType.ToString();
        choreCell = args.ChoreCell;
    }

    public void Execute() {
        new Thread(() => TryWithRetry()).Start();
    }

    private void TryWithRetry(int retries = 20) {
        const int retryDelayMs = 50;
        log.Level = retries > 0 ? LogLevel.Error : LogLevel.Info;
        bool choreFound = false;
        Container.Get<UnityTaskScheduler>().Run(
            () => {
                var choreContext = FindContext();
                if (choreContext != null) {
                    HostChores.Index[instanceId] = choreContext;
                    choreFound = true;
                }
            }
        );
         if (!choreFound && retries > 0) {
            Thread.Sleep(retryDelayMs);
            TryWithRetry(retries - 1);
        }
    }

    private Chore.Precondition.Context? FindContext() {
        var prefabID = Object.FindObjectsOfType<KPrefabID>().FirstOrDefault(a => a.InstanceID == instanceId);
        if (prefabID == null) {
            log.Warning($"Multiplayer: KPrefabID not found {instanceId}");
            return null;
        }
        var consumer = prefabID.GetComponent<ChoreConsumer>();
        if (consumer == null) {
            log.Warning(
                $"Multiplayer: Consumer does not exists at KPrefabId with desired ID {instanceId}. Id collision??"
            );
            return null;
        }
        if (instanceType != consumer.GetType().ToString()) {
            log.Warning(
                $"Multiplayer: Consumer type {consumer.GetType()} is not equal to the server {instanceType}."
            );
            return null;
        }

        return FindContext(consumer, choreId, choreCell, choreType);
    }

    private static Chore.Precondition.Context? FindContext(
        ChoreConsumer instance,
        int choreId,
        int cell,
        string serverChoreType
    ) {
        Chore choreWithIdCollision = null;
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

        if (choreWithIdCollision != null) {
            choreWithIdCollision.id = new Random().Next();
            choreWithIdCollision.driver = null;
        }

        if (chore == null) {
            log.Warning($"Multiplayer: Chore not found {choreId} {serverChoreType}");
            return null;
        }

        if (chore.id != choreId) {
            chore.id = choreId;
            log.Debug("Multiplayer: Corrected chore id.");
        }

        chore.driver = null;
        return new Chore.Precondition.Context(chore, instance.consumerState, true);
    }

    private static Chore FindInInstance(
        ChoreConsumer instance,
        int choreId,
        int cell,
        string serverChoreType,
        ref Chore choreWithIdCollision
    ) {
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

    private static Chore FindInGlobal(
        ChoreConsumer instance,
        int choreId,
        int cell,
        string serverChoreType,
        ref Chore choreWithIdCollision
    ) {
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
        log.Debug(
            $"Multiplayer: Chore global search. Result is {chore != null}. {instance.GetType()} {serverChoreType}"
        );
        return chore;
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

        if (result == null) return null;

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
