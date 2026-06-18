namespace MultiplayerMod.Multiplayer.World;

public record WorldSaveTransferProgress(
    string Name,
    long ReceivedBytes,
    long TotalBytes,
    int ReceivedChunks,
    int TotalChunks
) {
    public double Percent => TotalBytes <= 0 ? 100.0 : ReceivedBytes * 100.0 / TotalBytes;
}
