using System;

namespace MultiplayerMod.multiplayer.message
{
    [Serializable]
    public struct WorldSaveChunk
    {
        public int chunkIndex;
        public int totalChunks;
        public byte[] chunkData;
    }
}