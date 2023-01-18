using System.Collections.Generic;
using System.IO;
using System.Linq;
using MultiplayerMod.multiplayer.message;

namespace MultiplayerMod.multiplayer.effect
{
    public static class WorldSaver
    {
        public static List<WorldSaveChunk> SaveWorld()
        {
            var tempFilePath = Path.GetTempFileName();
            SaveLoader.Instance.Save(tempFilePath);

            var result = File.ReadAllBytes(tempFilePath);
            return SplitToChunks(result);
        }


        private static List<WorldSaveChunk> SplitToChunks(byte[] saveWorld)
        {
            const int maxMsgSize = 100 * 1024; // 100 kb
            var chunksCount = (saveWorld.Length - 1) / maxMsgSize + 1;
            var chunks = new List<WorldSaveChunk>();
            for (var i = 0; i < chunksCount; i++)
            {
                chunks.Add(new WorldSaveChunk
                {
                    chunkIndex = i,
                    totalChunks = chunksCount,
                    chunkData = saveWorld.Skip(i * maxMsgSize).Take(maxMsgSize).ToArray()
                });
            }

            return chunks;
        }
    }
}