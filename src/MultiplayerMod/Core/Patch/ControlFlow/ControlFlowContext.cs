namespace MultiplayerMod.Core.Patch.ControlFlow;

public class ControlFlowContext(int adviceStackDepth) {
    public readonly int AdviceStackDepth = adviceStackDepth;
}
