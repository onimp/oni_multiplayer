namespace MultiplayerMod.Core.Patch.ControlFlow;

public interface IDetourEvaluator {
    ExecutionFlow Evaluate(ControlFlowContext context);
}
