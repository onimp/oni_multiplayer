using System;
using MultiplayerMod.Multiplayer.Commands;

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
        context.Dependencies.Get<WorldSaveTransferManager>().Append(transferId, chunkIndex, data);
    }

}
