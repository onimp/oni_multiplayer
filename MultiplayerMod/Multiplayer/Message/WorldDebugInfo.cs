using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace MultiplayerMod.Multiplayer.Message
{
    static class Extensions
    {
        public static IEnumerator<T> GetEnumerator<T>(this IEnumerator<T> enumerator) => enumerator;
    }

    [Serializable]
    public struct WorldDebugInfo
    {
        public int worldFrame;
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
        public Dictionary<string, List<string>> StateMachineStates;

        private const int MaxBatchesCount = 128;

        public static unsafe WorldDebugInfo CurrentDebugInfo()
        {
            var stateMachines = Object.FindObjectsOfType<StateMachineController>();
            var stateMachineStates = stateMachines.ToDictionary(
                stateMachine => stateMachine + stateMachine.GetHashCode().ToString(),
                stateMachine =>
                {
                    var states = new List<string>();
                    foreach (var instance in stateMachine.GetEnumerator())
                    {
                        states.Add(instance.ToString());
                    }

                    return states;
                });

            return new WorldDebugInfo
            {
                worldFrame = GameClock.Instance.GetFrame(),
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
                StateMachineStates = stateMachineStates
            };
        }

        public int Compare(WorldDebugInfo other, bool printDebug)
        {
            if (printDebug)
                Debug.Log("Comparing world debug infos.");
            var errorsCount = 0;
            errorsCount += CompareValues(nameof(worldFrame), worldFrame, other.worldFrame, printDebug);
            errorsCount += cellsCount * CompareValues(nameof(cellsCount), cellsCount, other.cellsCount, printDebug);
            errorsCount += cellsCount *
                           CompareValues(nameof(elementIdxHashes), elementIdxHashes, other.elementIdxHashes,
                               printDebug);
            errorsCount += cellsCount *
                           CompareValues(nameof(temperatureHashes), temperatureHashes, other.temperatureHashes,
                               printDebug);
            errorsCount += cellsCount * CompareValues(nameof(radiationHashes), radiationHashes, other.radiationHashes,
                printDebug);
            errorsCount += cellsCount * CompareValues(nameof(massHashes), massHashes, other.massHashes, printDebug);
            errorsCount += cellsCount *
                           CompareValues(nameof(propertiesHashes), propertiesHashes, other.propertiesHashes,
                               printDebug);
            errorsCount += cellsCount *
                           CompareValues(nameof(strengthInfoHashes), strengthInfoHashes, other.strengthInfoHashes,
                               printDebug);
            errorsCount += cellsCount *
                           CompareValues(nameof(insulationHashes), insulationHashes, other.insulationHashes,
                               printDebug);
            errorsCount += cellsCount *
                           CompareValues(nameof(diseaseIdxHashes), diseaseIdxHashes, other.diseaseIdxHashes,
                               printDebug);
            errorsCount += cellsCount *
                           CompareValues(nameof(diseaseCountHashes), diseaseCountHashes, other.diseaseCountHashes,
                               printDebug);
            errorsCount += cellsCount * CompareValues(nameof(accumulatedFlowValuesHashes), accumulatedFlowValuesHashes,
                other.accumulatedFlowValuesHashes, printDebug);
            // TODO do something with it
            // errorsCount += CompareValues(nameof(choreProvidersHashes), choreProvidersHashes,
            //     other.choreProvidersHashes, printDebug);
            // errorsCount += CompareArrayOfDictionaries(ChoreProvidersChoresHashes, other.ChoreProvidersChoresHashes,
            //     printDebug);
            if (printDebug)
                Debug.Log($"Errors count is {errorsCount}");
            return errorsCount;
        }

        private static int Hash(Chore chore)
        {
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

        private static unsafe int[] HashBatches<T>(T* objects) where T : unmanaged
        {
            var arr = new T[Grid.CellCount];
            for (var i = 0; i < Grid.CellCount; i++)
            {
                arr[i] = objects[i];
            }

            var batchesCount = MaxBatchesCount;
            if (arr.Length < MaxBatchesCount) batchesCount = arr.Length;
            var res = new List<int>();
            for (var i = 0; i < batchesCount; i++)
            {
                var arr2 = arr.Skip(i * arr.Length / batchesCount).Take(arr.Length / batchesCount)
                    .ToArray();
                res.Add(arr2.Aggregate(0, (a, v) => CombineHashCodes(a, v.GetHashCode())));
            }

            return res.ToArray();
        }

        private static int CombineHashCodes(int h1, int h2)
        {
            return ((h1 << 5) + h1) ^ h2;
        }

        private int CompareValues<T>(string name, T a, T b, bool printDebug)
        {
            if (a.Equals(b))
            {
                return 0;
            }

            if (printDebug)
                Debug.Log($"{name} {a} != {b}");
            return 1;
        }

        private int CompareValues<T>(string name, T[] a, T[] b, bool printDebug)
        {
            var failures = Enumerable.Range(0, Math.Max(a.Length, b.Length)).Where(i =>
                a.Length <= i || b.Length <= i || !a[i].Equals(b[i])
            ).ToArray();

            if (failures.Length != 0 && printDebug)
            {
                Debug.Log(
                    $"{name} Ok: {a.Length - failures.Length}/{a.Length}. Failures at {JoinToString(failures)}");
            }

            return failures.Length;
        }

        private int CompareArrayOfDictionaries(Dictionary<int, int[]>[] a, Dictionary<int, int[]>[] b, bool printDebug)
        {
            var failures = Enumerable.Range(0, Math.Max(a.Length, b.Length)).Where(i =>
                a.Length <= i || b.Length <= i || !CompareDictionaries(a[i], b[i])
            ).ToArray();

            if (failures.Length != 0 && printDebug)
            {
                Debug.Log(
                    $"Chore dicts Ok: {a.Length - failures.Length}/{a.Length}. Failures at {JoinToString(failures.ToArray())}");
            }

            return failures.Length;
        }

        private static bool CompareDictionaries(Dictionary<int, int[]> mapA, Dictionary<int, int[]> mapB)
        {
            return mapA.Keys.SequenceEqual(mapB.Keys) && mapA.Keys.All(key => mapA[key].SequenceEqual(mapB[key]));
        }


        private static string JoinToString<T>(T[] objects)
        {
            return String.Join(", ", objects);
        }
    }
}
