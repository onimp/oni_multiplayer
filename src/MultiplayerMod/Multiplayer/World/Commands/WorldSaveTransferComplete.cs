using System;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.UI.Overlays;

namespace MultiplayerMod.Multiplayer.World.Commands;

[Serializable]
[MultiplayerCommand(Type = MultiplayerCommandType.System)]
public class WorldSaveTransferComplete : MultiplayerCommand {

    private readonly Guid transferId;

    public WorldSaveTransferComplete(Guid transferId) {
        this.transferId = transferId;
    }

    public override void Execute(MultiplayerCommandContext context) {
        MultiplayerStatusOverlay.Text = "Verifying world save...";
        var world = context.Dependencies.Get<WorldSaveTransferManager>().Complete(transferId);
        context.Dependencies.Get<WorldManager>().RequestWorldLoad(world);
    }

}
