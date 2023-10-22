using System;

namespace MultiplayerMod.Multiplayer.Commands.Debug;

[Serializable]
public class DebugSimulationStep : MultiplayerCommand {

    public override void Execute(MultiplayerCommandContext context) => global::Game.Instance.ForceSimStep();

}
