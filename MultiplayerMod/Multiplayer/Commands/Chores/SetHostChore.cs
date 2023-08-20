using System;
using System.Collections.Generic;
using System.Linq;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.Multiplayer.Objects.Reference;
using MultiplayerMod.Multiplayer.World;
using Object = UnityEngine.Object;

namespace MultiplayerMod.Multiplayer.Commands.Chores;

[Serializable]
public class SetHostChore : MultiplayerCommand {

    private static Core.Logging.Logger log = LoggerFactory.GetLogger<SetHostChore>();

    private ComponentReference<ChoreConsumer> reference;
    private int instanceCell;
    private int choreId;
    private string choreType;
    private int choreCell;
    private bool isAttemptingOverride;

    public SetHostChore(FindNextChoreEventArgs args) {
        log.Level = LogLevel.Debug;

        reference = args.Reference;
        instanceCell = args.InstanceCell;
        choreId = args.ChoreId;
        choreType = args.ChoreType.ToString();
        choreCell = args.ChoreCell;
        isAttemptingOverride = args.IsAttemptingOverride;
    }

    public override void Execute() {
        log.Debug(
            $"Received {reference} {instanceCell} {choreId} {choreType} {choreCell}"
        );

        if (!HostChores.Index.ContainsKey(reference))
            HostChores.Index[reference] = new Queue<Func<Chore.Precondition.Context?>>();
        HostChores.Index[reference].Enqueue(FindContext);
    }

    private Chore.Precondition.Context? FindContext() {
        var consumer = reference.GetComponent();
        var localCell = Grid.PosToCell(consumer.transform.position);
        if (instanceCell != localCell) {
            // TODO add warning/sync on too big difference only.
            log.Warning(
                $"Multiplayer: Consumer {consumer}-{choreType} found but in different cell. Server {instanceCell} - local {localCell}."
            );
        }

        return FindContext(consumer, choreId, choreCell, choreType, isAttemptingOverride);
    }

    private static Chore.Precondition.Context? FindContext(
        ChoreConsumer instance,
        int choreId,
        int cell,
        string serverChoreType,
        bool isAttemptingOverride
    ) {
        Chore? choreWithIdCollision = null;
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
            log.Warning($"Multiplayer: Chore not found {instance} - id#{choreId}. Type-{serverChoreType}");
            return null;
        }

        log.Level = LogLevel.Debug;

        if (chore.id != choreId) {
            chore.id = choreId;
            log.Trace($"Multiplayer: Corrected {instance}-{serverChoreType} chore id.");
        }

        chore.driver = null;
        log.Debug($"Found {instance}-{serverChoreType} {choreId}-{cell}");
        return new Chore.Precondition.Context(chore, instance.consumerState, isAttemptingOverride);
    }

    private static Chore? FindInInstance(
        ChoreConsumer instance,
        int choreId,
        int cell,
        string serverChoreType,
        ref Chore? choreWithIdCollision
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
        ) ?? FindByTypeAndCell(chores, cell, serverChoreType);
    }

    private static Chore? FindInGlobal(
        ChoreConsumer instance,
        int choreId,
        int cell,
        string serverChoreType,
        ref Chore? choreWithIdCollision
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
        ) ?? FindByTypeAndCell(globalChores, cell, serverChoreType);
        log.Debug(
            $"Multiplayer: Chore global search. Result is {chore != null}. {instance.GetType()} {serverChoreType}"
        );
        return chore;
    }

    private static Chore? FindFullMatch(
        Chore[] chores,
        int choreId,
        int choreCell,
        string choreType,
        ref Chore? choreWithIdCollision
    ) {
        var result = chores.FirstOrDefault(chore => chore.id == choreId);

        if (result == null) return null;

        var clientChoreCell = Grid.PosToCell(result.gameObject.transform.position);
        if (!DependsOnConsumerCell(choreType) && choreCell != clientChoreCell) {
            log.Warning(
                $"Multiplayer: Server chore pos != client chore pos. {choreId}. {choreType}. {choreCell} != {clientChoreCell}"
            );
            choreWithIdCollision = result;
            return null;
        }
        if (result.GetType().ToString() == choreType)
            return result;

        log.Warning(
            $"Chore type is not equal client: {result.GetType()} != server {choreType}"
        );
        choreWithIdCollision = result;
        return null;
    }

    private static Chore? FindByTypeAndCell(Chore[] chores, int choreCell, string choreType) {
        var choreOfType = chores.Where(chore => chore.GetType().ToString() == choreType).ToList();
        var results = choreOfType.Where(
            chore => DependsOnConsumerCell(choreType) ||
                     choreCell == Grid.PosToCell(chore.gameObject.transform.position)
        ).ToArray();
        if (results.Length == 0) {
            var cellPoses = string.Join(
                ", ",
                choreOfType.Select(chore => Grid.PosToCell(chore.gameObject.transform.position))
            );
            log.Debug(
                $"FindByTypeAndCell : Not found {choreType}(cell={choreCell}) in total chores {chores.Length}. Chores of type {choreOfType.Count}."
            );
            log.Debug($"FindByTypeAndCell: Positions of typed chores: {cellPoses}");
            return null;
        }
        if (results.Length > 1) {
            var cellPoses = string.Join(
                ", ",
                results.Select(chore => Grid.PosToCell(chore.gameObject.transform.position))
            );
            log.Warning(
                $"FindByTypeAndCell : Not single {choreType} in total chores {chores.Length}. Matches of type {results.Length}."
            );
            log.Warning($"FindByTypeAndCell: Positions of results chores: {cellPoses}");
            return null;
        }
        return results.Single();
    }

    /// <summary>
    /// This chores depends only on consumer position.
    ///
    /// If consumer position is off due to any reason chore must be taken regardless of its position.
    /// </summary>
    /// <returns></returns>
    private static bool DependsOnConsumerCell(string choreType) {
        string[] independent = { "IdleChore", "MoveToSafetyChore" };
        return independent.Any(choreType.Contains);
    }

}
