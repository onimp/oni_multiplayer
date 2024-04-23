using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using JetBrains.Annotations;

namespace MultiplayerMod.Core.Patch;

[Dependency.Dependency, UsedImplicitly]
public class ControlFlowCustomizer(Harmony harmony) {

    private static readonly ConditionalWeakTable<object, Dictionary<MethodInfo, object?>> extents = new();
    private static readonly HashSet<MethodInfo> patchedMethods = [];

    public void DisableMethod<T>(T instance, MethodInfo method) where T : notnull => DisableMethod<T, object?>(
        instance,
        method,
        method.ReturnType.IsValueType
            ? method.ReturnType != typeof(void) ? Activator.CreateInstance(method.ReturnType) : null
            : null
    );

    public void DisableMethod<T, R>(T instance, MethodInfo method, R? returnValue = default) where T : notnull {
        if (!patchedMethods.Contains(method)) {
            harmony.CreateProcessor(method)
                .AddPrefix(ResolveAdvice(method))
                .Patch();
            patchedMethods.Add(method);
        }
        extents.GetOrCreateValue(instance).Add(method, returnValue);
    }

    public void EnableMethods(object instance) => extents.Remove(instance);

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
        if (!extents.TryGetValue(__instance, out var methods))
            return true;

        return !methods.ContainsKey(__originalMethod);
    }

    [UsedImplicitly]
    private static bool AdviceWithRet<R>(MethodInfo __originalMethod, object __instance, ref R? __result) {
        if (!extents.TryGetValue(__instance, out var methods))
            return true;

        var skip = methods.TryGetValue(__originalMethod, out var returnValue);
        if (skip)
            __result = (R?) returnValue;
        return !skip;
    }

}
