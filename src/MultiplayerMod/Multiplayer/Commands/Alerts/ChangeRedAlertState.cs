using System;

namespace MultiplayerMod.Multiplayer.Commands.Alerts;

[Serializable]
public class ChangeRedAlertState : MultiplayerCommand {

    private bool enabled;

    public ChangeRedAlertState(bool enabled) {
        this.enabled = enabled;
    }

    public override void Execute(MultiplayerCommandContext context) {
        ClusterManager.Instance.activeWorld.AlertManager.ToggleRedAlert(enabled);
    }

}
