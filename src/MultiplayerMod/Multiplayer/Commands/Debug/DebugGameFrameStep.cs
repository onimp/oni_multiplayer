using System;

namespace MultiplayerMod.Multiplayer.Commands.Debug;

[Serializable]
public class DebugGameFrameStep : MultiplayerCommand {

    public override void Execute(MultiplayerCommandContext context) => SpeedControlScreen.Instance.DebugStepFrame();

}
