namespace MultiplayerMod.Core.Patch.ControlFlow;

public interface IDetourEvaluator {
    ControlAdviceBehavior Evaluate(ControlFlowContext context);
}
