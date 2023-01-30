using System.Collections.Generic;
using System.IO;
using MultiplayerMod.multiplayer.message;

namespace MultiplayerMod.multiplayer.effect
{
    public static class WorldLoader
    {
        private static readonly Dictionary<int, WorldSaveChunk> SaveChunks = new Dictionary<int, WorldSaveChunk>();

        public static void LoadWorld(object obj)
        {
            var chunk = (WorldSaveChunk)obj;
            Debug.Log($"ClientActions.LoadWorld chunk {chunk.chunkIndex + 1}/{chunk.totalChunks}");
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
        }
    }
}