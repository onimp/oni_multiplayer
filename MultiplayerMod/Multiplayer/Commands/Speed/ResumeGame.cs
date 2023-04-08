using System;

namespace MultiplayerMod.Multiplayer.Commands.Speed;

[Serializable]
public class ResumeGame : IMultiplayerCommand {

    public void Execute() {
        if (SpeedControlScreen.Instance.IsPaused)
            SpeedControlScreen.Instance.Unpause();
    }

}
