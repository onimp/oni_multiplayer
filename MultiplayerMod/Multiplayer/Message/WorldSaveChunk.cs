using System;

namespace MultiplayerMod.Multiplayer.Message
{
    [Serializable]
    public struct WorldSaveChunk
    {
        public int chunkIndex;
        public int totalChunks;
        public byte[] chunkData;
    }
}
