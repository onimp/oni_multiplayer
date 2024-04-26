using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using JetBrains.Annotations;

namespace MultiplayerMod.Core.Patch.ControlFlow;

[Dependency.Dependency, UsedImplicitly]
public class ControlFlowCustomizer(Harmony harmony) {

    private class ControlBlock {
        public object? ReturnValue;
        public IDetourEvaluator? Evaluator;
    }

    private static readonly ConditionalWeakTable<object, Dictionary<MethodInfo, ControlBlock>> blocks = new();
    private static readonly HashSet<MethodInfo> patchedMethods = [];

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

    private HarmonyMethod ResolveAdvice(MethodInfo method) {
        if (method.ReturnType == typeof(void))
            return new HarmonyMethod(AccessTools.Method(typeof(ControlFlowCustomizer), nameof(Advice)));

        return new HarmonyMethod(
            AccessTools
                .Method(typeof(ControlFlowCustomizer), nameof(AdviceWithRet))
                .MakeGenericMethod(method.ReturnType)
        );
    }

    [UsedImplicitly]
    private static bool Advice(MethodInfo __originalMethod, object __instance) {
        return Resolve(__originalMethod, __instance, out _) == ControlAdviceBehavior.Invoke;
    }

    [UsedImplicitly]
    private static bool AdviceWithRet<R>(MethodInfo __originalMethod, object __instance, ref R? __result) {
        if (Resolve(__originalMethod, __instance, out var returnValue) == ControlAdviceBehavior.Invoke)
            return true;

        __result = (R?) returnValue;
        return false;
    }

    private static readonly ControlFlowContext context = new(2);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static ControlAdviceBehavior Resolve(
        MethodInfo __originalMethod,
        object __instance,
        out object? returnValue
    ) {
        returnValue = null;

        if (!blocks.TryGetValue(__instance, out var methods))
            return ControlAdviceBehavior.Invoke;

        if (!methods.TryGetValue(__originalMethod, out var controlBlock))
            return ControlAdviceBehavior.Invoke;

        if (controlBlock.Evaluator?.Evaluate(context) == ControlAdviceBehavior.Invoke)
            return ControlAdviceBehavior.Invoke;

        returnValue = controlBlock.ReturnValue;
        return ControlAdviceBehavior.Detour;
    }

}
