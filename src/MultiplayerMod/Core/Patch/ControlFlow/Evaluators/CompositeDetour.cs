using System.Linq;
using static MultiplayerMod.Core.Patch.ControlFlow.ExecutionFlow;

namespace MultiplayerMod.Core.Patch.ControlFlow.Evaluators;

public class CompositeDetour(params IDetourEvaluator[] evaluators) : IDetourEvaluator {

    public ExecutionFlow Evaluate(ControlFlowContext context) {
        var newContext = new ControlFlowContext(context.AdviceStackDepth + 3);
        return evaluators.Any(evaluator => evaluator.Evaluate(newContext) == Continue) ? Continue : Break;
    }

}
