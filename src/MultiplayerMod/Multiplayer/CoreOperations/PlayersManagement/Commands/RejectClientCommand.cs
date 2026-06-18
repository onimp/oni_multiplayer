using System;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.UI.Overlays;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.CoreOperations.PlayersManagement.Commands;

[Serializable]
[MultiplayerCommand(Type = MultiplayerCommandType.System)]
public class RejectClientCommand : MultiplayerCommand {

    private readonly string reason;

    public RejectClientCommand(string reason) {
        this.reason = reason;
    }

    public override void Execute(MultiplayerCommandContext context) {
        MultiplayerStatusOverlay.Show($"Multiplayer compatibility check failed:\n{reason}");
        context.Dependencies.Get<IMultiplayerClient>().Disconnect();
    }

}
