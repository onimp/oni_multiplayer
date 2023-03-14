using System.Collections.Generic;
using System.IO;
using System.Linq;
using MultiplayerMod.Multiplayer.Message;
using MultiplayerMod.patch;

namespace MultiplayerMod.Multiplayer.Effect
{
    public static class WorldSaver
    {
        public static List<WorldSaveChunk> SaveWorld()
        {
            var tempFilePath = SaveLoader.GetAutosaveFilePath();
            SaveLoaderPatch.DisablePatch = true;
            SaveLoader.Instance.Save(tempFilePath);
            SaveLoaderPatch.DisablePatch = false;
            return ReadWorldSave(tempFilePath);
        }

        public static List<WorldSaveChunk> ReadWorldSave(string saveFileName)
        {
            var result = File.ReadAllBytes(saveFileName);
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
