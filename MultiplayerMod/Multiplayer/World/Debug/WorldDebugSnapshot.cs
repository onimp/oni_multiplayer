using System;
using System.Collections.Generic;
using System.Linq;
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
    Dictionary<int, int[]>[] ChoreProvidersChoresHashes
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
            ).ToArray()
        );
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
        hash = CombineHashCodes(hash, chore.addToDailyReport.GetHashCode());
        return CombineHashCodes(hash, chore.reportType.GetHashCode());
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
