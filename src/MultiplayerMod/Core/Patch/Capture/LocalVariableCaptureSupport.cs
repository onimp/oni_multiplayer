using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using HarmonyLib;
using MonoMod.Utils.Cil;

namespace MultiplayerMod.Core.Patch.Capture;

[HarmonyPatch]
[HarmonyPriority(Priority.First)]
public static class LocalVariableCaptureSupport {

    public static readonly ThreadLocal<LocalCaptureContainer?> ContainerReference = new();

    private static readonly Dictionary<MethodInfo, Type> captureTypes;

    static LocalVariableCaptureSupport() {
        captureTypes = AccessTools.GetTypesFromAssembly(typeof(LocalVariableCaptureSupport).Assembly)
            .Select(type => (type, attributes: type.GetCustomAttributes<LocalVariableCaptureAttribute>()))
            .Where(it => it.attributes.Any())
            .SelectMany(
                it => {
                    var (type, attributes) = it;
                    return attributes.Select(
                        attribute => (
                            type,
                            method: AccessTools.Method(
                                attribute.DeclaringType,
                                attribute.MethodName,
                                attribute.ArgumentTypes
                            )
                        )
                    );
                }
            )
            .ToDictionary(it => it.method, it => it.type);
    }

    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<MethodBase> TargetMethods() => captureTypes.Keys;

    // ReSharper disable once UnusedMember.Local
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> InjectCaptureSupport(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator,
        MethodBase method
    ) {
        using var source = instructions.GetEnumerator();
        var result = new List<CodeInstruction>();

        var gen = (CecilILGenerator) generator.GetProxiedShim();
        var variables = gen._Variables.Keys.ToDictionary(it => it.LocalIndex);

        var captureType = captureTypes[(MethodInfo) method];
        var assignedVariables = new Dictionary<int, PropertyInfo>();

        foreach (var property in captureType.GetProperties()) {
            var attribute = property.GetCustomAttribute<LocalVariableAttribute>();
            if (attribute == null)
                continue;

            if (!variables.TryGetValue(attribute.Index, out var variable))
                throw new InvalidLocalVariableReferenceException(
                    $"Local variable with index {attribute.Index} not found in {method}"
                );

            if (variable.LocalType != property.PropertyType)
                throw new InvalidLocalVariableReferenceException(
                    $"Local variable with index {attribute.Index} in {method} has a type {variable.LocalType}, " +
                    $"but required type is {property.PropertyType}."
                );

            assignedVariables[variable.LocalIndex] = property;
        }

        var captureVariable = generator.DeclareLocal(captureType);
        var containerVariable = generator.DeclareLocal(typeof(LocalCaptureContainer));

        // Inject capture initialization before each return statement
        while (result.AddConditional(source, it => it.opcode == OpCodes.Ret, false)) {
            var returnLabel = generator.DefineLabel();
            var returnInstruction = source.Current!;

            // Check if the container (ContainerReference.Value) is available
            // Also rewire original ret labels
            result.Add(
                CodeInstruction
                    .LoadField(typeof(LocalVariableCaptureSupport), nameof(ContainerReference))
                    .WithLabels(returnInstruction.labels)
            );
            result.Add(
                new CodeInstruction(
                    OpCodes.Callvirt,
                    AccessTools.PropertyGetter(
                        typeof(ThreadLocal<LocalCaptureContainer>),
                        nameof(ThreadLocal<LocalCaptureContainer>.Value)
                    )
                )
            );

            result.Add(new CodeInstruction(OpCodes.Stloc, containerVariable));
            result.Add(new CodeInstruction(OpCodes.Ldloc, containerVariable));
            result.Add(new CodeInstruction(OpCodes.Ldnull));
            result.Add(new CodeInstruction(OpCodes.Cgt_Un));

            // If the container is unavailable proceed to the return statement
            result.Add(new CodeInstruction(OpCodes.Brfalse_S, returnLabel));

            // Create a new capture and add it to the container
            result.Add(new CodeInstruction(OpCodes.Newobj, captureType.GetConstructor(Type.EmptyTypes)));
            result.Add(new CodeInstruction(OpCodes.Stloc, captureVariable));
            result.Add(new CodeInstruction(OpCodes.Ldloc, containerVariable));
            result.Add(
                new CodeInstruction(
                    OpCodes.Callvirt,
                    AccessTools.PropertyGetter(typeof(LocalCaptureContainer), nameof(LocalCaptureContainer.Captures))
                )
            );
            result.Add(new CodeInstruction(OpCodes.Ldloc, captureVariable));
            result.Add(
                new CodeInstruction(
                    OpCodes.Callvirt,
                    AccessTools.Method(typeof(List<object>), nameof(List<object>.Add))
                )
            );

            // Initialize capture properties with the given local variables
            foreach (var (index, property) in assignedVariables) {
                result.Add(new CodeInstruction(OpCodes.Ldloc, captureVariable));
                result.Add(new CodeInstruction(OpCodes.Ldloc, variables[index]));
                result.Add(new CodeInstruction(OpCodes.Callvirt, property.SetMethod));
            }

            // Add the original ret instruction with the new label
            result.Add(returnInstruction.Clone().WithLabels(returnLabel));
        }

        return result;
    }

}
