using System;

namespace MultiplayerMod.multiplayer
{
    [Serializable]
    public struct WorldSaveChunk
    {
        public int chunkIndex;
        public int totalChunks;
        public byte[] chunkData;
    }
}