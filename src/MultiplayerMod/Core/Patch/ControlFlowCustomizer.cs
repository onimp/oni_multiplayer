using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Events;

namespace MultiplayerMod.Core.Patch;

[Core.Dependency.Dependency, UsedImplicitly]
public class ControlFlowCustomizer {

    private readonly Harmony harmony;

    public ControlFlowCustomizer(EventDispatcher events, Harmony harmony) {
        this.harmony = harmony;
    }

    private static readonly ConditionalWeakTable<object, Dictionary<MethodInfo, object?>> extents = new();
    private static readonly HashSet<MethodInfo> patchedMethods = [];

    public void DisableMethod<T>(T instance, MethodInfo method) where T : notnull =>
        DisableMethod<T, object?>(instance, method);

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
    private static bool AdviceWithRet<T>(MethodInfo __originalMethod, T __instance, ref T? __result) where T : notnull {
        if (!extents.TryGetValue(__instance, out var methods))
            return true;

        var skip = methods.TryGetValue(__originalMethod, out var returnValue);
        if (skip)
            __result = (T?) returnValue;
        return !skip;
    }

}
