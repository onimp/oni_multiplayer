using System;

namespace MultiplayerMod.Multiplayer.Commands.Speed;

[Serializable]
public class PauseGame : MultiplayerCommand {

    public override void Execute(MultiplayerCommandContext context) {
        if (!SpeedControlScreen.Instance.IsPaused)
            SpeedControlScreen.Instance.Pause();
    }

}
