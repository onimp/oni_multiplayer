using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Patch.Generics;

namespace MultiplayerMod.Core.Patch.ControlFlow;

[Dependency.Dependency, UsedImplicitly]
public class ControlFlowCustomizer(Harmony harmony) {

    private class ControlBlock {
        public object? ReturnValue;
        public IDetourEvaluator? Evaluator;
    }

    private static readonly ConditionalWeakTable<object, Dictionary<MethodInfo, ControlBlock>> blocks = new();
    private static readonly HashSet<MethodInfo> patchedMethods = [];
    private static readonly MethodInfo adviceMethod = AccessTools.Method(typeof(ControlFlowCustomizer), nameof(Advice));
    private static readonly MethodInfo adviceRetMethod = AccessTools.Method(typeof(ControlFlowCustomizer), nameof(AdviceWithRet));

    public void Detour<T>(T instance, MethodInfo method, IDetourEvaluator? evaluator = null) where T : notnull {
        var defaultValue = method.ReturnType.IsValueType
            ? method.ReturnType != typeof(void) ? Activator.CreateInstance(method.ReturnType) : null
            : null;
        Detour<T, object?>(instance, method, defaultValue, evaluator);
    }

    public void Detour<T, R>(
        T instance,
        MethodInfo method,
        R? returnValue = default,
        IDetourEvaluator? evaluator = null
    ) where T : notnull {
        InjectAdvice(method);
        var block = new ControlBlock { ReturnValue = returnValue, Evaluator = evaluator };
        blocks.GetOrCreateValue(instance).Add(method, block);
    }

    private void InjectAdvice(MethodInfo method) {
        if (patchedMethods.Contains(method))
            return;

        harmony.CreateProcessor(method).AddPrefix(ResolveAdvice(method)).Patch();
        patchedMethods.Add(method);
    }

    public void Reset(object instance) => blocks.Remove(instance);

    private static HarmonyMethod ResolveAdvice(MethodInfo method) => method.ReturnType == typeof(void)
        ? new HarmonyMethod(adviceMethod)
        : new HarmonyMethod(adviceRetMethod);

    [UsedImplicitly]
    private static bool Advice(MethodInfo __originalMethod, object __instance, object[] __args) {
        if (ResolveFlow(__originalMethod, __instance, out _) == ExecutionFlow.Continue)
            return !HarmonyGenericsRouter.TryRoute(__originalMethod, __instance, __args);
        return false;
    }

    [UsedImplicitly]
    private static bool AdviceWithRet(
        MethodInfo __originalMethod,
        object __instance,
        object[] __args,
        ref object? __result
    ) {
        if (ResolveFlow(__originalMethod, __instance, out var returnValue) == ExecutionFlow.Continue)
            return !HarmonyGenericsRouter.TryRoute(__originalMethod, __instance, __args, ref __result);

        __result = returnValue;
        return false;
    }

    private static readonly ControlFlowContext context = new(2);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static ExecutionFlow ResolveFlow(MethodInfo __originalMethod, object __instance, out object? returnValue) {
        returnValue = null;

        if (!blocks.TryGetValue(__instance, out var methods))
            return ExecutionFlow.Continue;

        if (!methods.TryGetValue(__originalMethod, out var controlBlock))
            return ExecutionFlow.Continue;

        if (controlBlock.Evaluator?.Evaluate(context) == ExecutionFlow.Continue)
            return ExecutionFlow.Continue;

        returnValue = controlBlock.ReturnValue;
        return ExecutionFlow.Break;
    }

}
