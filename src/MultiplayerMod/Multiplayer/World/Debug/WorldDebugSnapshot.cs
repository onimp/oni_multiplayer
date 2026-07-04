using System;
using System.Collections.Generic;
using System.Linq;
using MultiplayerMod.Multiplayer.Objects;
using Object = UnityEngine.Object;

namespace MultiplayerMod.Multiplayer.World.Debug;

[Serializable]
public record WorldDebugSnapshot(
    float WorldTime,
    int CellsCount,
    int[] ElementIdxHashes,
    int[] TemperatureHashes,
    int[] RadiationHashes,
    int[] MassHashes,
    int[] PropertiesHashes,
    int[] StrengthInfoHashes,
    int[] InsulationHashes,
    int[] DiseaseIdxHashes,
    int[] DiseaseCountHashes,
    int[] AccumulatedFlowValuesHashes,
    Dictionary<string, string[]> StateMachineStates,
    int[] ChoreProvidersHashes,
    Dictionary<int, int[]>[] ChoreProvidersChoresHashes,
    DuplicantSnapshot[] Duplicants
) {

    public const int MaxBatchesCount = 128;

    public static unsafe WorldDebugSnapshot Create() {
        var stateMachines = Object.FindObjectsOfType<StateMachineController>();
        var stateMachineStates = stateMachines.ToDictionary(
            stateMachine => stateMachine + stateMachine.GetHashCode().ToString(),
            stateMachine => {
                var states = new List<string>();
                foreach (var instance in stateMachine) {
                    states.Add(instance.ToString());
                }
                return states.ToArray();
            }
        );
        var choreProviders = Object.FindObjectsOfType<ChoreProvider>();
        return new WorldDebugSnapshot(
            GameClock.Instance.GetTime(),
            Grid.CellCount,
            HashBatches(Grid.elementIdx),
            HashBatches(Grid.temperature),
            HashBatches(Grid.radiation),
            HashBatches(Grid.mass),
            HashBatches(Grid.properties),
            HashBatches(Grid.strengthInfo),
            HashBatches(Grid.insulation),
            HashBatches(Grid.diseaseIdx),
            HashBatches(Grid.diseaseCount),
            HashBatches(Grid.AccumulatedFlowValues),
            stateMachineStates,
            choreProviders.Select(choreProvider => choreProvider.Name.GetHashCode()).ToArray(),
            choreProviders.Select(
                choreProvider => choreProvider.choreWorldMap.ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.Select(Hash).ToArray()
                )
            ).ToArray(),
            CreateDuplicantSnapshots()
        );
    }

    public static DuplicantSnapshot[] CreateDuplicantSnapshots() {
        return global::Components.LiveMinionIdentities.Items
            .Where(identity => identity != null)
            .Select(identity => {
                var gameObject = identity.gameObject;
                var chore = gameObject.GetComponent<ChoreDriver>()?.GetCurrentChore();
                var navigator = gameObject.GetComponent<Navigator>();
                return new DuplicantSnapshot(
                    gameObject.GetComponent<MultiplayerInstance>()?.Id,
                    gameObject.GetProperName(),
                    Grid.PosToCell(gameObject),
                    chore == null ? "-" : chore.GetType().Name,
                    chore is StandardChoreBase standardChore
                        ? standardChore.GetSMI()?.GetCurrentState()?.name ?? ""
                        : "",
                    navigator != null && navigator.targetLocator != null
                        ? Grid.PosToCell(navigator.targetLocator)
                        : Grid.InvalidCell
                );
            })
            .ToArray();
    }

    private static int Hash(Chore chore) {
        var hash = 0;
        hash = CombineHashCodes(hash, chore.choreType.Name.GetHashCode());
        hash = CombineHashCodes(hash, chore.target.name.GetHashCode());
        hash = CombineHashCodes(hash, chore.runUntilComplete.GetHashCode());
        hash = CombineHashCodes(hash, chore.masterPriority.priority_class.GetHashCode());
        hash = CombineHashCodes(hash, chore.masterPriority.priority_value.GetHashCode());
        hash = CombineHashCodes(hash, chore.IsPreemptable.GetHashCode());
        hash = CombineHashCodes(hash, chore.priorityMod.GetHashCode());
        return CombineHashCodes(hash, chore.GetReportType().GetHashCode());
    }

    private static unsafe int[] HashBatches<T>(T* objects) where T : unmanaged {
        var arr = new T[Grid.CellCount];
        for (var i = 0; i < Grid.CellCount; i++) {
            arr[i] = objects[i];
        }
        var batchesCount = MaxBatchesCount;
        if (arr.Length < MaxBatchesCount) batchesCount = arr.Length;
        var res = new List<int>();
        for (var i = 0; i < batchesCount; i++) {
            var arr2 = arr.Skip(i * arr.Length / batchesCount).Take(arr.Length / batchesCount).ToArray();
            res.Add(arr2.Aggregate(0, (a, v) => CombineHashCodes(a, v.GetHashCode())));
        }
        return res.ToArray();
    }

    private static int CombineHashCodes(int h1, int h2) {
        return ((h1 << 5) + h1) ^ h2;
    }
}

// Per-duplicant debug record used by the duplicant sync inspector (DevToolDuplicantSync).
// Keyed by MultiplayerId so host and client rows can be joined; the remaining fields are the
// observable state that tends to drift (position, current chore, chore state, nav target).
[Serializable]
public record DuplicantSnapshot(
    MultiplayerId? Id,
    string Name,
    int Cell,
    string ChoreType,
    string ChoreState,
    int NavTargetCell
);
