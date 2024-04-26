using System.Linq;
using static MultiplayerMod.Core.Patch.ControlFlow.ControlAdviceBehavior;

namespace MultiplayerMod.Core.Patch.ControlFlow.Evaluators;

public class CompositeDetour(params IDetourEvaluator[] evaluators) : IDetourEvaluator {

    public ControlAdviceBehavior Evaluate(ControlFlowContext context) {
        var newContext = new ControlFlowContext(context.AdviceStackDepth + 3);
        return evaluators.Any(evaluator => evaluator.Evaluate(newContext) == Invoke) ? Invoke : Detour;
    }

}
