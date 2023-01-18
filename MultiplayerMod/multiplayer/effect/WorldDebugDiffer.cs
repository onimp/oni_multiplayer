using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Object = UnityEngine.Object;

namespace MultiplayerMod.multiplayer.effect
{
    public static class WorldDebugDiffer
    {
        public static void CalculateWorldSummary()
        {
            Debug.Log($"CalculateWorldSummary frame {GameClock.Instance.GetFrame()}");
            unsafe
            {
                Debug.Log($"Cells count {Grid.CellCount}");
                // Some of values are 0 for some reason.
                LogArrayHash("elementIdx", Grid.elementIdx);
                LogArrayHash("temperature", Grid.temperature);
                LogArrayHash("radiation", Grid.radiation);
                LogArrayHash("mass", Grid.mass);
                LogArrayHash("properties", Grid.properties);
                LogArrayHash("strengthInfo", Grid.strengthInfo);
                LogArrayHash("insulation", Grid.insulation);
                LogArrayHash("diseaseIdx", Grid.diseaseIdx);
                LogArrayHash("diseaseCount", Grid.diseaseCount);
                LogArrayHash("AccumulatedFlowValues", Grid.AccumulatedFlowValues);
                // LogArrayHash(PropertyTextures.externalFlowTex);
                //  LogArrayHash(PropertyTextures.externalLiquidTex);
                //    LogArrayHash(PropertyTextures.externalExposedToSunlight);

                // TODO chores are empty by default.
                // 
                var objects = Object.FindObjectsOfType<ChoreProvider>();
                Debug.Log($"Found chore providers {objects.Length}");
                foreach (var choreProvider in objects)
                {
                    Debug.Log(choreProvider);
                    var array = choreProvider.choreWorldMap.Values.Select(a => a.Count).ToArray();
                    Debug.Log(
                        $"{choreProvider.choreWorldMap.Count} / [{JoinToString(array)}]");
                    LogArrayHash("Chore", choreProvider.choreWorldMap.Values.ToArray());
                }
            }
        }

        private static unsafe void LogArrayHash(string name, float* array)
        {
            var arr = new float[Grid.CellCount];
            Marshal.Copy((IntPtr)array, arr, 0, Grid.CellCount);
            var hash = arr.Aggregate(0, (a, v) => CombineHashCodes(a, v.GetHashCode()));
            Debug.Log($"{name} hash is {hash}");
            Debug.Log($"{name} hash batches are {HashBatches(arr)}");
        }

        private static unsafe void LogArrayHash(string name, ushort* array)
        {
            var arr = new short[Grid.CellCount];
            Marshal.Copy((IntPtr)array, arr, 0, Grid.CellCount);
            var hash = arr.Aggregate(0, (a, v) => CombineHashCodes(a, v.GetHashCode()));
            Debug.Log($"{name} hash is {hash}");
            Debug.Log($"{name} hash batches are {HashBatches(arr)}");
        }

        private static unsafe void LogArrayHash(string name, byte* array)
        {
            var arr = new byte[Grid.CellCount];
            Marshal.Copy((IntPtr)array, arr, 0, Grid.CellCount);
            var hash = arr.Aggregate(0, (a, v) => CombineHashCodes(a, v.GetHashCode()));
            Debug.Log($"{name} hash is {hash}");
            Debug.Log($"{name} hash batches are {HashBatches(arr)}");
        }

        private static unsafe void LogArrayHash(string name, int* array)
        {
            var arr = new int[Grid.CellCount];
            Marshal.Copy((IntPtr)array, arr, 0, Grid.CellCount);
            var hash = arr.Aggregate(0, (a, v) => CombineHashCodes(a, v.GetHashCode()));
            Debug.Log($"{name} hash is {hash}");
            Debug.Log($"{name} hash batches are {HashBatches(arr)}");
        }
        private static void LogArrayHash(string name, List<Chore>[] array)
        {
            var hash = array.Aggregate(0, (a, v) => CombineHashCodes(a, v.GetHashCode()));
            Debug.Log($"{name} hash is {hash}");
            Debug.Log($"{name} hash batches are {HashBatches(array)}");
        }

        private static string HashBatches<T>(T[] objects)
        {
            var batchesCount = 16;
            if (objects.Length < batchesCount) batchesCount = objects.Length; 
            var res = new List<int>();
            for (var i = 0; i < batchesCount; i++)
            {
                var arr = objects.Skip(i * objects.Length / batchesCount).Take(objects.Length / batchesCount).ToArray();
                res.Add(arr.Aggregate(0, (a, v) => CombineHashCodes(a, v.GetHashCode())));
            }

            return JoinToString(res.ToArray());
        }


        private static string JoinToString<T>(T[] objects)
        {
            return String.Join(", ", objects);
        }

        private static int CombineHashCodes(int h1, int h2)
        {
            return ((h1 << 5) + h1) ^ h2;
        }
    }
}