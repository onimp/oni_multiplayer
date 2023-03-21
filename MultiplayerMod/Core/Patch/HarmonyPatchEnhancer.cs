using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using HarmonyLib;

namespace MultiplayerMod.Core.Patch;

[HarmonyPatch]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
public class HarmonyPatchEnhancer {

    private static readonly Regex applicableMethods = new(
        @"prefix|postfix",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    private static bool methodsFound;

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    private static IEnumerable<MethodBase> TargetMethods() {
        var methods = typeof(HarmonyPatchEnhancer).Assembly.GetTypes()
            .Where(type => type.GetCustomAttribute<EnhancedPatchAttribute>() != null)
            .SelectMany(type => AccessTools.GetDeclaredMethods(type).Where(ShouldPatchMethod));

        methodsFound = methods.Any();
        return methods;
    }

    private static bool Prepare(MethodBase original) {
        return original != null || methodsFound;
    }

    private static bool Prefix(ref bool __result) {
        if (!PatchEnhancer.PatchesDisabled())
            return true;

        __result = true;
        return false;
    }

    private static bool ShouldPatchMethod(MethodInfo method) {
        if (method.GetCustomAttribute<DisableableAttribute>() != null)
            return true;

        return applicableMethods.Match(method.Name).Success;
    }

}
