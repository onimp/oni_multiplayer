using System;

namespace MultiplayerMod.Multiplayer.Commands.Speed;

[Serializable]
public class PauseGame : IMultiplayerCommand {

    public void Execute() {
        if (!SpeedControlScreen.Instance.IsPaused)
            SpeedControlScreen.Instance.Pause();
    }

}
