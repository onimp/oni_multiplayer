using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace MultiplayerMod.multiplayer.message
{
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
        public int[] choreProvidersHashes;
        public Dictionary<int, int[]>[] ChoreProvidersChoresHashes;

        public static unsafe WorldDebugInfo CurrentDebugInfo()
        {
            var objects = Object.FindObjectsOfType<ChoreProvider>();

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
                choreProvidersHashes = objects.Select(choreProvider => choreProvider.Name.GetHashCode()).ToArray(),
                ChoreProvidersChoresHashes = objects.Select(choreProvider =>
                    choreProvider.choreWorldMap.ToDictionary(pair => pair.Key,
                        pair => pair.Value.Select(Hash).ToArray())).ToArray()
            };
        }

        public void Compare(WorldDebugInfo other)
        {
            Debug.Log("Comparing world debug infos.");
            CompareValues(nameof(worldFrame), worldFrame, other.worldFrame);
            CompareValues(nameof(cellsCount), cellsCount, other.cellsCount);
            CompareValues(nameof(elementIdxHashes), elementIdxHashes, other.elementIdxHashes);
            CompareValues(nameof(temperatureHashes), temperatureHashes, other.temperatureHashes);
            CompareValues(nameof(radiationHashes), radiationHashes, other.radiationHashes);
            CompareValues(nameof(massHashes), massHashes, other.massHashes);
            CompareValues(nameof(propertiesHashes), propertiesHashes, other.propertiesHashes);
            CompareValues(nameof(strengthInfoHashes), strengthInfoHashes, other.strengthInfoHashes);
            CompareValues(nameof(insulationHashes), insulationHashes, other.insulationHashes);
            CompareValues(nameof(diseaseIdxHashes), diseaseIdxHashes, other.diseaseIdxHashes);
            CompareValues(nameof(diseaseCountHashes), diseaseCountHashes, other.diseaseCountHashes);
            CompareValues(nameof(accumulatedFlowValuesHashes), accumulatedFlowValuesHashes,
                other.accumulatedFlowValuesHashes);
            CompareValues(nameof(choreProvidersHashes), choreProvidersHashes, other.choreProvidersHashes);
            CompareArrayOfDictionaries(ChoreProvidersChoresHashes, other.ChoreProvidersChoresHashes);
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

            var batchesCount = 16;
            if (arr.Length < batchesCount) batchesCount = arr.Length;
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

        private void CompareValues<T>(string name, T a, T b)
        {
            if (a.Equals(b))
            {
                return;
            }

            Debug.Log($"{name} {a} != {b}");
        }

        private void CompareValues<T>(string name, T[] a, T[] b)
        {
            if (a.SequenceEqual(b))
            {
                return;
            }

            var failures = Enumerable.Range(0, a.Length).Where(i => !a[i].Equals(b[i])).ToArray();

            Debug.Log(
                $"{name} Ok: {a.Length - failures.Length}/{a.Length}. Failures at {JoinToString(failures)}");
        }

        private void CompareArrayOfDictionaries(Dictionary<int, int[]>[] a, Dictionary<int, int[]>[] b)
        {
            var failures = Enumerable.Range(0, a.Length).Where(i => !CompareDictionaries(a[i], b[i])).ToArray();

            if (failures.Length == 0)
            {
                return;
            }

            Debug.Log(
                $"Chore dicts Ok: {a.Length - failures.Length}/{a.Length}. Failures at {JoinToString(failures.ToArray())}");
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