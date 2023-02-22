using System.Collections.Generic;
using System.IO;
using System.Threading;
using MultiplayerMod.multiplayer.message;
using MultiplayerMod.patch;

namespace MultiplayerMod.multiplayer.effect
{
    public static class WorldLoader
    {
        private static readonly Dictionary<int, WorldSaveChunk> SaveChunks = new();
        private static readonly Mutex LoadMutex = new();

        public static void StartLoading()
        {
            LoadMutex.WaitOne();

            LoadingOverlay.Load(() =>
            {
                LoadMutex.WaitOne();
                LoadMutex.ReleaseMutex();
            });
        }

        public static void LoadWorldChunk(object obj)
        {
            if (LoadMutex.WaitOne(1))
            {
                LoadMutex.ReleaseMutex();
                StartLoading();
            }

            var chunk = (WorldSaveChunk)obj;
            SaveChunks[chunk.chunkIndex] = chunk;
            if (chunk.chunkIndex + 1 != chunk.totalChunks)
                return;

            if (SaveChunks.Count != chunk.totalChunks)
            {
                Debug.Log("Some load chunks have been lost.");
                SaveChunks.Clear();
                return;
            }

            var tempFilePath = Path.GetTempFileName();
            using (var writer = new BinaryWriter(File.OpenWrite(tempFilePath)))
            {
                for (var i = 0; i < chunk.totalChunks; i++)
                {
                    writer.Write(SaveChunks[i].chunkData);
                }
            }

            SaveChunks.Clear();

            typeof(LoadScreen).InvokePrivateStatic("DoLoad", new[] { typeof(string) }, tempFilePath);
            LoadMutex.ReleaseMutex();
        }
    }
}