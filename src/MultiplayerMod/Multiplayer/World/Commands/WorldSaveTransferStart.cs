using System;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.UI.Overlays;
using MultiplayerMod.Multiplayer.World.Data;

namespace MultiplayerMod.Multiplayer.World.Commands;

[Serializable]
[MultiplayerCommand(Type = MultiplayerCommandType.System)]
public class WorldSaveTransferStart : MultiplayerCommand {

    private readonly Guid transferId;
    private readonly string name;
    private readonly WorldState state;
    private readonly long totalBytes;
    private readonly int chunkSize;
    private readonly int chunkCount;
    private readonly string sha256;

    public WorldSaveTransferStart(
        Guid transferId,
        string name,
        WorldState state,
        long totalBytes,
        int chunkSize,
        int chunkCount,
        string sha256
    ) {
        this.transferId = transferId;
        this.name = name;
        this.state = state;
        this.totalBytes = totalBytes;
        this.chunkSize = chunkSize;
        this.chunkCount = chunkCount;
        this.sha256 = sha256;
    }

    public override void Execute(MultiplayerCommandContext context) {
        context.Dependencies.Get<WorldSaveTransferManager>()
            .Start(transferId, name, state, totalBytes, chunkSize, chunkCount, sha256);
        MultiplayerStatusOverlay.Show($"Receiving world save {name}...\n0/{chunkCount} chunks, {FormatBytes(totalBytes)} total");
    }

    private static string FormatBytes(long bytes) {
        var mib = bytes / 1024.0 / 1024.0;
        return $"{mib:0.0} MiB";
    }

}
