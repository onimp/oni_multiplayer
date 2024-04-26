using System.Diagnostics;
using System.Reflection;
using HarmonyLib;
using static MultiplayerMod.Core.Patch.ControlFlow.ControlAdviceBehavior;

namespace MultiplayerMod.Core.Patch.ControlFlow.Evaluators;

public class MethodBoundedDetour(MethodBase method) : IDetourEvaluator {

    public ControlAdviceBehavior Evaluate(ControlFlowContext context) {
        var stack = new StackTrace();
        var caller = Harmony.GetOriginalMethodFromStackframe(stack.GetFrame(context.AdviceStackDepth + 2));
        return caller == method ? Detour : Invoke;
    }

}
