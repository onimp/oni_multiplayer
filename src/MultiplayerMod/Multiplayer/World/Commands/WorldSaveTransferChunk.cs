using System;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.UI.Overlays;

namespace MultiplayerMod.Multiplayer.World.Commands;

[Serializable]
[MultiplayerCommand(Type = MultiplayerCommandType.System)]
public class WorldSaveTransferChunk : MultiplayerCommand {

    private readonly Guid transferId;
    private readonly int chunkIndex;
    private readonly byte[] data;

    public WorldSaveTransferChunk(Guid transferId, int chunkIndex, byte[] data) {
        this.transferId = transferId;
        this.chunkIndex = chunkIndex;
        this.data = data;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var manager = context.Dependencies.Get<WorldSaveTransferManager>();
        manager.Append(transferId, chunkIndex, data);
        var progress = manager.GetProgress(transferId);
        MultiplayerStatusOverlay.Text =
            $"Receiving world save {progress.Name}...\n" +
            $"{progress.ReceivedChunks}/{progress.TotalChunks} chunks ({progress.Percent:0.0}%)";
    }

}
