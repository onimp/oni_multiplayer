using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Extensions;

namespace MultiplayerMod.Core.Patch.Generics;

[HarmonyManual]
[HarmonyPatch]
public static class HarmonyGenericsRouter {

    private static readonly ConcurrentDictionary<MethodBase, MethodBase> replacements = new();

    [UsedImplicitly]
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PatchTools), nameof(PatchTools.RememberObject))]
    private static void RememberObject(object key, object value) {
        if (key is not MethodBase original || value is not MethodBase replacement)
            return;

        replacements[original] = replacement;
    }

    /// <summary>
    /// Try to route an instance method call to the appropriate generic patch
    /// </summary>
    /// <param name="original">An original method of a generic type</param>
    /// <param name="instance">An instance of a target generic type</param>
    /// <param name="args">Method invocation arguments</param>
    /// <exception cref="HarmonyGenericsRouterException">Unable to route (see the message for a reason)</exception>
    /// <returns><b>true</b> - The invocation was routed to the correct generic type method<br/></returns>
    /// <returns><b>false</b> - Current invocation has the correct generic type method</returns>
    public static bool TryRoute(MethodBase original, object instance, object[] args) {
        var method = RewriteGenericPatch(original, instance);
        if (method == original)
            return false;

        method.Invoke(instance, args);
        return true;
    }

    /// <summary>
    /// Try to route an instance method call to the appropriate generic patch
    /// </summary>
    /// <param name="original">An original method of the current generic type</param>
    /// <param name="instance">An instance of the target generic type</param>
    /// <param name="args">Method invocation arguments</param>
    /// <param name="result">Invocation result: <b>present only if the invocation was routed</b></param>
    /// <returns><b>true</b> - The invocation was routed to the correct generic type method<br/></returns>
    /// <returns><b>false</b> - Current invocation has the correct generic type method</returns>
    /// <exception cref="HarmonyGenericsRouterException">Unable to route (see the message for a reason)</exception>
    public static bool TryRoute(MethodBase original, object instance, object[] args, ref object? result) {
        var method = RewriteGenericPatch(original, instance);
        if (method == original)
            return false;

        result = method.Invoke(instance, args);
        return true;
    }

    /// <summary>
    /// Rewrites a generic type method patch with the correct generic method patch for the <paramref name="instance"/>.
    /// </summary>
    /// <param name="original">Original method associated with the current patch</param>
    /// <param name="instance">Instance of the target generic type</param>
    /// <returns>The same method as <paramref name="original"/>, but with the generic types of the <paramref name="instance"/></returns>
    /// <exception cref="HarmonyGenericsRouterException">Unable to route (see the message for a reason)</exception>
    private static MethodBase RewriteGenericPatch(MethodBase original, object instance) {
        var currentType = instance.GetType();
        var patchedType = original.DeclaringType!;

        if (patchedType.IsAssignableFrom(currentType))
            return original;

        var patchedGeneric = patchedType.IsGenericType ? patchedType.GetGenericTypeDefinition() : null;
        if (patchedGeneric == null)
            throw new HarmonyGenericsRouterException($"Only patched generic types are supported. Type: {patchedType}");

        var targetGenericType = FindGenericAncestor(currentType, patchedGeneric);
        if (targetGenericType == null)
            throw new HarmonyGenericsRouterException(
                $"Unable to find target generic type of {instance.GetType()} matching {patchedGeneric}."
            );

        var targetMethod = FindSameMethod(original, targetGenericType);
        replacements.TryGetValue(targetMethod, out var replacement);

        if (replacement == null) {
            lock (PatchProcessor.locker) {
                var patchInfo = HarmonySharedState.GetPatchInfo(original) ?? new PatchInfo();
                HarmonySharedState.UpdatePatchInfo(
                    targetMethod,
                    PatchFunctions.UpdateWrapper(targetMethod, patchInfo),
                    patchInfo
                );
            }
        } else {
            var message = Memory.DetourMethod(targetMethod, replacement);
            if (message != null)
                throw new HarmonyGenericsRouterException(message);
        }

        return targetMethod;
    }

    private static MethodInfo FindSameMethod(MethodBase original, Type targetGenericType) {
        var ptr = Memory.GetMethodStart(original, out _);
        var method = targetGenericType.GetAllMethods()
            .Where(it => it.Name == original.Name)
            .First(it => Memory.GetMethodStart(it, out _) == ptr);
        return method;
    }

    private static Type? FindGenericAncestor(Type type, Type genericDefinition) {
        var currentType = type;
        while (currentType != null && currentType != typeof(object)) {
            if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == genericDefinition)
                return currentType;

            currentType = currentType.BaseType;
        }
        return null;
    }

}
