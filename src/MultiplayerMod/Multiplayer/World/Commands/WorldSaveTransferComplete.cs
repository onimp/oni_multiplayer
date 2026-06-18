using System;
using MultiplayerMod.Multiplayer.Commands;

namespace MultiplayerMod.Multiplayer.World.Commands;

[Serializable]
[MultiplayerCommand(Type = MultiplayerCommandType.System)]
public class WorldSaveTransferComplete : MultiplayerCommand {

    private readonly Guid transferId;

    public WorldSaveTransferComplete(Guid transferId) {
        this.transferId = transferId;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var world = context.Dependencies.Get<WorldSaveTransferManager>().Complete(transferId);
        context.Dependencies.Get<WorldManager>().RequestWorldLoad(world);
    }

}
