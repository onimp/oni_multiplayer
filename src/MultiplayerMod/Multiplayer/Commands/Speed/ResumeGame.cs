using System;

namespace MultiplayerMod.Multiplayer.Commands.Speed;

[Serializable]
public class ResumeGame : MultiplayerCommand {

    public override void Execute(MultiplayerCommandContext context) {
        if (SpeedControlScreen.Instance.IsPaused)
            SpeedControlScreen.Instance.Unpause();
    }

}
