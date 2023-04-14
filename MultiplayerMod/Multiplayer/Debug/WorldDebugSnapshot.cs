using System;
using System.Collections.Generic;
using System.Linq;
using MultiplayerMod.Core.Logging;
using Object = UnityEngine.Object;

namespace MultiplayerMod.Multiplayer.Debug;

[Serializable]
public class WorldDebugSnapshot {
    private static Core.Logging.Logger log = LoggerFactory.GetLogger<WorldDebugSnapshot>();

    public float worldTime;
    public int cellsCount;
    public int[] elementIdxHashes;
    public int[] temperatureHashes;
    public int[] radiationHashes;
    public int[] massHashes;
    public int[] propertiesHashes;
    public int[] strengthInfoHashes;
    public int[] insulationHashes;
    public int[] diseaseIdxHashes;
    public int[] diseaseCountHashes;
    public int[] accumulatedFlowValuesHashes;
    public Dictionary<string, string[]> StateMachineStates;
    public int[] choreProvidersHashes;
    public Dictionary<int, int[]>[] ChoreProvidersChoresHashes;
    private const int maxBatchesCount = 128;

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
        return new WorldDebugSnapshot {
            worldTime = GameClock.Instance.GetTime(),
            cellsCount = Grid.CellCount,
            elementIdxHashes = HashBatches(Grid.elementIdx),
            temperatureHashes = HashBatches(Grid.temperature),
            radiationHashes = HashBatches(Grid.radiation),
            massHashes = HashBatches(Grid.mass),
            propertiesHashes = HashBatches(Grid.properties),
            strengthInfoHashes = HashBatches(Grid.strengthInfo),
            insulationHashes = HashBatches(Grid.insulation),
            diseaseIdxHashes = HashBatches(Grid.diseaseIdx),
            diseaseCountHashes = HashBatches(Grid.diseaseCount),
            accumulatedFlowValuesHashes = HashBatches(Grid.AccumulatedFlowValues),
            StateMachineStates = stateMachineStates,
            choreProvidersHashes = choreProviders.Select(choreProvider => choreProvider.Name.GetHashCode()).ToArray(),
            ChoreProvidersChoresHashes = choreProviders.Select(
                choreProvider => choreProvider.choreWorldMap.ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.Select(Hash).ToArray()
                )
            ).ToArray()
        };
    }

    public int Compare(WorldDebugSnapshot other, bool printDebug) {
        log.Level = printDebug ? LogLevel.Debug : LogLevel.Info;
        log.Debug("Comparing world debug infos.");
        var errorsCount = 0;
        var cellsPerBatch = cellsCount / maxBatchesCount;
        errorsCount += CompareValues(nameof(worldTime), worldTime, other.worldTime);
        errorsCount += cellsCount * CompareValues(nameof(cellsCount), cellsCount, other.cellsCount);
        errorsCount += cellsPerBatch * CompareValues(
            nameof(elementIdxHashes),
            elementIdxHashes,
            other.elementIdxHashes
        );
        errorsCount += cellsPerBatch * CompareValues(
            nameof(temperatureHashes),
            temperatureHashes,
            other.temperatureHashes
        );
        errorsCount += cellsPerBatch * CompareValues(nameof(radiationHashes), radiationHashes, other.radiationHashes);
        errorsCount += cellsPerBatch * CompareValues(nameof(massHashes), massHashes, other.massHashes);
        errorsCount += cellsPerBatch * CompareValues(
            nameof(propertiesHashes),
            propertiesHashes,
            other.propertiesHashes
        );
        errorsCount += cellsPerBatch * CompareValues(
            nameof(strengthInfoHashes),
            strengthInfoHashes,
            other.strengthInfoHashes
        );
        errorsCount += cellsPerBatch * CompareValues(
            nameof(insulationHashes),
            insulationHashes,
            other.insulationHashes
        );
        errorsCount += cellsPerBatch * CompareValues(
            nameof(diseaseIdxHashes),
            diseaseIdxHashes,
            other.diseaseIdxHashes
        );
        errorsCount += cellsPerBatch * CompareValues(
            nameof(diseaseCountHashes),
            diseaseCountHashes,
            other.diseaseCountHashes
        );
        errorsCount += cellsPerBatch * CompareValues(
            nameof(accumulatedFlowValuesHashes),
            accumulatedFlowValuesHashes,
            other.accumulatedFlowValuesHashes
        );
        errorsCount += CompareDictionaries(StateMachineStates, other.StateMachineStates);
        errorsCount += CompareValues(nameof(choreProvidersHashes), choreProvidersHashes, other.choreProvidersHashes);
        errorsCount += CompareArrayOfDictionaries(ChoreProvidersChoresHashes, other.ChoreProvidersChoresHashes);
        log.Debug($"Errors count is {errorsCount}");
        return errorsCount;
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
        var batchesCount = maxBatchesCount;
        if (arr.Length < maxBatchesCount) batchesCount = arr.Length;
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

    private int CompareValues<T>(string name, T a, T b) {
        if (a.Equals(b)) return 0;

        log.Debug($"{name} {a} != {b}");
        return 1;
    }

    private int CompareValues<T>(string name, T[] a, T[] b) {
        var failures = Enumerable.Range(0, Math.Max(a.Length, b.Length))
            .Where(i => a.Length <= i || b.Length <= i || !a[i].Equals(b[i])).ToArray();
        if (failures.Length != 0)
            log.Debug($"{name} Ok: {a.Length - failures.Length}/{a.Length}. Failures at {JoinToString(failures)}");
        return failures.Length;
    }

    private int CompareArrayOfDictionaries(Dictionary<int, int[]>[] a, Dictionary<int, int[]>[] b) {
        var failures = Enumerable.Range(0, Math.Max(a.Length, b.Length))
            .Where(i => a.Length <= i || b.Length <= i || !CompareDictionaries(a[i], b[i])).ToArray();
        if (failures.Length != 0) {
            log.Debug(
                $"Chore dicts Ok: {a.Length - failures.Length}/{a.Length}. Failures at {JoinToString(failures.ToArray())}"
            );
        }
        return failures.Length;
    }

    private int CompareDictionaries(Dictionary<string, string[]> a, Dictionary<string, string[]> b) {
        const int failuresForMissingKey = 100;
        var keys = a.Keys.ToList();
        keys.AddRange(b.Keys);
        keys = keys.Distinct().ToList();
        var res = 0;
        foreach (var key in keys) {
            if (!a.ContainsKey(key) || !b.ContainsKey(key)) {
                res += failuresForMissingKey;
                continue;
            }
            res += CompareValues(key, a[key], b[key]);
        }
        return res;
    }

    private static bool CompareDictionaries(Dictionary<int, int[]> mapA, Dictionary<int, int[]> mapB) {
        return mapA.Keys.SequenceEqual(mapB.Keys) && mapA.Keys.All(key => mapA[key].SequenceEqual(mapB[key]));
    }

    private static string JoinToString<T>(T[] objects) {
        return String.Join(", ", objects);
    }
}
