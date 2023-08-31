using System;
using MultiplayerMod.ModRuntime;

namespace MultiplayerMod.Multiplayer.Commands.Speed;

[Serializable]
public class PauseGame : MultiplayerCommand {

    public override void Execute(Runtime runtime) {
        if (!SpeedControlScreen.Instance.IsPaused)
            SpeedControlScreen.Instance.Pause();
    }

}
