using System;
using MultiplayerMod.Game.World;

namespace MultiplayerMod.Multiplayer.Commands.Alerts;

[Serializable]
public class ChangeRedAlertState : MultiplayerCommand {

    private bool enabled;
    private int? worldId;

    public ChangeRedAlertState(bool enabled, int? worldId = null) {
        this.enabled = enabled;
        this.worldId = worldId;
    }

    public override void Execute(MultiplayerCommandContext context) {
        WorldIdentity.ToggleRedAlert(WorldIdentity.GetWorld(worldId), enabled);
    }

}
