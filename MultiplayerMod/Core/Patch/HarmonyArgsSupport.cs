using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Logging;

namespace MultiplayerMod.Core.Patch;

/// <summary>
/// This is a patch for Harmony version 2.0.5.0 that adds a new version feature.
/// If the library will be updated in game this patch must be removed.
/// </summary>
// ReSharper disable once UnusedType.Global
[HarmonyPatch(typeof(MethodPatcher))]
[HarmonyPriority(Priority.First)]
public static class HarmonyArgsSupport {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger(typeof(HarmonyArgsSupport));

    private const string argsParameterName = "__args";
    private const string supportedVersion = "2.0.5.0";

    // ReSharper disable once UnusedMember.Local
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(MethodPatcher.CreateReplacement))]
    private static IEnumerable<CodeInstruction> CreateReplacement(IEnumerable<CodeInstruction> instructions) {
        if (!HarmonyVersionIsSupported())
            return instructions;

        var accessor = new CodeInstructionsOffsetAccessor(instructions.ToList());

        var result = new List<CodeInstruction>();
        result.AddRange(accessor.GetInstructions(0x0000, 0x0067));

        // HarmonyArgsSupport.InjectArgsVariable
        result.Add(new CodeInstruction(OpCodes.Ldarg_0));
        result.Add(new CodeInstruction(OpCodes.Ldloc_0));
        result.Add(accessor.GetInstruction(0x004e).Clone());
        result.Add(CodeInstruction.Call(typeof(HarmonyArgsSupport), nameof(InjectArgsVariable)));

        result.AddRange(accessor.GetInstructions(0x0069, 0x05ab));

        return result;
    }

    // ReSharper disable once UnusedMember.Local
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(MethodPatcher.EmitCallParameter))]
    private static IEnumerable<CodeInstruction> EmitCallParameter(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator il
    ) {
        if (!HarmonyVersionIsSupported())
            return instructions;

        var accessor = new CodeInstructionsOffsetAccessor(instructions.ToList());
        var result = new List<CodeInstruction>();
        result.AddRange(accessor.GetInstructions(0x0000, 0x00ae));

        var label = il.DefineLabel();
        accessor.GetInstruction(0x00b0).labels.Add(label);
        result.Add(accessor.GetInstruction(0x00b0).Clone());
        result.Add(accessor.GetInstruction(0x00b2).Clone());
        result.Add(new CodeInstruction(OpCodes.Ldstr, argsParameterName));
        result.Add(accessor.GetInstruction(0x00bc).Clone());
        result.Add(new CodeInstruction(OpCodes.Brfalse_S, label));

        result.Add(new CodeInstruction(OpCodes.Ldarg_0));
        result.Add(new CodeInstruction(OpCodes.Ldarg_2));
        result.Add(CodeInstruction.Call(typeof(HarmonyArgsSupport), nameof(InjectParamArgs)));
        result.Add(accessor.GetInstruction(0x00de).Clone());

        result.AddRange(accessor.GetInstructions(0x00b0, 0x09b2));

        return result;
    }

    private static bool HarmonyVersionIsSupported() {
        var attribute = typeof(Harmony).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        var version = attribute.InformationalVersion;
        var supported = version == supportedVersion;
        if (!supported)
            log.Warning(
                $"""
                 Harmony __args support may be broken: Harmony version {version} isn't supported.
                 Supported version is {supportedVersion}).
                 """
            );
        return supported;
    }

    // The following code is adapted from Harmony v2.2.2.0
    // https://github.com/pardeike/Harmony/blob/v2.2.2.0/Harmony/Internal/MethodPatcher.cs

    private static void InjectParamArgs(MethodPatcher instance, Dictionary<string, LocalBuilder> variables) {
        if (variables.TryGetValue(argsParameterName, out var argsArrayVar))
            instance.emitter.Emit(OpCodes.Ldloc, argsArrayVar);
        else
            instance.emitter.Emit(OpCodes.Ldnull);
    }

    private static void InjectArgsVariable(MethodPatcher instance, Dictionary<string, LocalBuilder> privateVars) {
        var fixes = instance.prefixes.Union(instance.postfixes).ToList();
        if (!fixes.Any(fix => fix.GetParameters().Any(p => p.Name == argsParameterName)))
            return;

        PrepareArgumentArray(instance);
        var argsArrayVariable = instance.il.DeclareLocal(typeof(object[]));
        instance.emitter.Emit(OpCodes.Stloc, argsArrayVariable);
        privateVars[argsParameterName] = argsArrayVariable;
    }

    private static void PrepareArgumentArray(MethodPatcher instance) {
        var original = instance.original;
        var emitter = instance.emitter;
        var parameters = original.GetParameters();
        var argumentShift = original.IsStatic ? 0 : 1;

        parameters.ForEachIndexed(
            (index, parameter) => {
                if (parameter.IsOut || parameter.IsRetval)
                    InitializeOutParameter(instance, index + argumentShift, parameter.ParameterType);
            }
        );

        emitter.Emit(OpCodes.Ldc_I4, parameters.Length);
        emitter.Emit(OpCodes.Newarr, typeof(object));

        parameters.ForEachIndexed(
            (index, parameter) => {
                var type = parameter.ParameterType;
                var paramByRef = type.IsByRef;
                if (paramByRef)
                    type = type.GetElementType()!;
                emitter.Emit(OpCodes.Dup);
                emitter.Emit(OpCodes.Ldc_I4, index);
                emitter.Emit(OpCodes.Ldarg, index + argumentShift);
                if (paramByRef) {
                    if (AccessTools.IsStruct(type))
                        emitter.Emit(OpCodes.Ldobj, type);
                    else
                        emitter.Emit(MethodPatcher.LoadIndOpCodeFor(type));
                }
                if (type.IsValueType)
                    emitter.Emit(OpCodes.Box, type);
                emitter.Emit(OpCodes.Stelem_Ref);
            }
        );
    }

    private static void InitializeOutParameter(MethodPatcher instance, int argumentIndex, Type type) {
        var emitter = instance.emitter;
        if (type.IsByRef)
            type = type.GetElementType()!;
        emitter.Emit(OpCodes.Ldarg, argumentIndex);
        if (AccessTools.IsStruct(type)) {
            emitter.Emit(OpCodes.Initobj, type);
            return;
        }
        if (AccessTools.IsValue(type)) {
            if (type == typeof(float)) {
                emitter.Emit(OpCodes.Ldc_R4, (float) 0);
                emitter.Emit(OpCodes.Stind_R4);
                return;
            }
            if (type == typeof(double)) {
                emitter.Emit(OpCodes.Ldc_R8, (double) 0);
                emitter.Emit(OpCodes.Stind_R8);
                return;
            }
            if (type == typeof(long)) {
                emitter.Emit(OpCodes.Ldc_I8, (long) 0);
                emitter.Emit(OpCodes.Stind_I8);
                return;
            }
            emitter.Emit(OpCodes.Ldc_I4, 0);
            emitter.Emit(OpCodes.Stind_I4);
            return;
        }
        emitter.Emit(OpCodes.Ldnull);
        emitter.Emit(OpCodes.Stind_Ref);
    }

}
