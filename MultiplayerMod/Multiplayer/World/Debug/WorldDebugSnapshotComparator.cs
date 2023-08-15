using System;
using System.Collections.Generic;
using System.Linq;
using MultiplayerMod.Core.Logging;

namespace MultiplayerMod.Multiplayer.World.Debug;

public static class WorldDebugSnapshotComparator {
    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger(typeof(WorldDebugSnapshotComparator));

    public static int Compare(WorldDebugSnapshot snapshot, WorldDebugSnapshot other, bool printDebug) {
        log.Level = printDebug ? LogLevel.Debug : LogLevel.Info;
        log.Debug("Comparing world debug infos.");
        var errorsCount = 0;
        var cellsPerBatch = snapshot.CellsCount / WorldDebugSnapshot.MaxBatchesCount;
        errorsCount += CompareValues(nameof(snapshot.WorldTime), snapshot.WorldTime, other.WorldTime);
        errorsCount += snapshot.CellsCount * CompareValues(
            nameof(snapshot.CellsCount),
            snapshot.CellsCount,
            other.CellsCount
        );
        errorsCount += cellsPerBatch * CompareValues(
            nameof(snapshot.ElementIdxHashes),
            snapshot.ElementIdxHashes,
            other.ElementIdxHashes
        );
        errorsCount += cellsPerBatch * CompareValues(
            nameof(snapshot.TemperatureHashes),
            snapshot.TemperatureHashes,
            other.TemperatureHashes
        );
        errorsCount += cellsPerBatch * CompareValues(
            nameof(snapshot.RadiationHashes),
            snapshot.RadiationHashes,
            other.RadiationHashes
        );
        errorsCount += cellsPerBatch * CompareValues(
            nameof(snapshot.MassHashes),
            snapshot.MassHashes,
            other.MassHashes
        );
        errorsCount += cellsPerBatch * CompareValues(
            nameof(snapshot.PropertiesHashes),
            snapshot.PropertiesHashes,
            other.PropertiesHashes
        );
        errorsCount += cellsPerBatch * CompareValues(
            nameof(snapshot.StrengthInfoHashes),
            snapshot.StrengthInfoHashes,
            other.StrengthInfoHashes
        );
        errorsCount += cellsPerBatch * CompareValues(
            nameof(snapshot.InsulationHashes),
            snapshot.InsulationHashes,
            other.InsulationHashes
        );
        errorsCount += cellsPerBatch * CompareValues(
            nameof(snapshot.DiseaseIdxHashes),
            snapshot.DiseaseIdxHashes,
            other.DiseaseIdxHashes
        );
        errorsCount += cellsPerBatch * CompareValues(
            nameof(snapshot.DiseaseCountHashes),
            snapshot.DiseaseCountHashes,
            other.DiseaseCountHashes
        );
        errorsCount += cellsPerBatch * CompareValues(
            nameof(snapshot.AccumulatedFlowValuesHashes),
            snapshot.AccumulatedFlowValuesHashes,
            other.AccumulatedFlowValuesHashes
        );
        errorsCount += CompareDictionaries(snapshot.StateMachineStates, other.StateMachineStates);
        errorsCount += CompareValues(
            nameof(snapshot.ChoreProvidersHashes),
            snapshot.ChoreProvidersHashes,
            other.ChoreProvidersHashes
        );
        errorsCount += CompareArrayOfDictionaries(
            snapshot.ChoreProvidersChoresHashes,
            other.ChoreProvidersChoresHashes
        );
        log.Debug($"Errors count is {errorsCount}");
        return errorsCount;
    }

    private static int CompareValues<T>(string name, T a, T b) where T : IComparable {
        if (a.Equals(b)) return 0;

        log.Debug($"{name} {a} != {b}");
        return 1;
    }

    private static int CompareValues<T>(string name, T[] a, T[] b) where T : IComparable {
        var failures = Enumerable.Range(0, Math.Max(a.Length, b.Length))
            .Where(i => a.Length <= i || b.Length <= i || !a[i].Equals(b[i])).ToArray();
        if (failures.Length != 0)
            log.Debug($"{name} Ok: {a.Length - failures.Length}/{a.Length}. Failures at {JoinToString(failures)}");
        return failures.Length;
    }

    private static int CompareArrayOfDictionaries(Dictionary<int, int[]>[] a, Dictionary<int, int[]>[] b) {
        var failures = Enumerable.Range(0, Math.Max(a.Length, b.Length))
            .Where(i => a.Length <= i || b.Length <= i || !CompareDictionaries(a[i], b[i])).ToArray();
        if (failures.Length != 0) {
            log.Debug(
                $"Chore dictionaries Ok: {a.Length - failures.Length}/{a.Length}. Failures at {JoinToString(failures.ToArray())}"
            );
        }
        return failures.Length;
    }

    private static int CompareDictionaries(Dictionary<string, string[]> a, Dictionary<string, string[]> b) {
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
        return string.Join(", ", objects);
    }
}
