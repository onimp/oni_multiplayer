using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Microsoft.Build.Utilities;
using MultiplayerMod.AttributeProcessor.MSBuild.Task.Extensions;

namespace MultiplayerMod.AttributeProcessor.MSBuild.Task.Processors;

public class ConditionalInvocationProcessor : IAttributeProcessor {

    private readonly ModuleDefMD module;
    private readonly TaskLoggingHelper log;

    public ConditionalInvocationProcessor(ModuleDefMD module, TaskLoggingHelper log) {
        this.module = module;
        this.log = log;
    }

    public bool Process() {
        try {
            InjectPredicates(CollectPredicates());
        } catch (AttributeContractException exception) {
            log.LogError(exception.Message);
            return false;
        }
        return true;
    }

    private Dictionary<TypeDef, MethodDef> CollectPredicates() {
        var result = new Dictionary<TypeDef, MethodDef>();
        foreach (var type in module.GetTypes()) {
            var attribute = type.CustomAttributes.Find(typeof(ConditionalInvocationAttribute).FullName);
            if (attribute == null)
                continue;

            AssertAttributeContract(type);
            var conditionMethod = GetConditionMethod(attribute);
            result[type] = conditionMethod;
            type.CustomAttributes.Remove(attribute);
        }
        return result;
    }

    private static void AssertAttributeContract(TypeDef attributeType) {
        if (attributeType.BaseType.FullName != "System.Attribute")
            throw new AttributeContractException(
                $"Attribute \"{nameof(ConditionalInvocationAttribute)}\" " +
                $"is only valid on classes derived from System.Attribute"
            );

        var usage = attributeType.CustomAttributes.Find(typeof(AttributeUsageAttribute).FullName);
        if (usage == null || (int) usage.ConstructorArguments[0].Value != (int) AttributeTargets.Method)
            throw new AttributeContractException(
                $"\"{nameof(ConditionalInvocationAttribute)}\" supports attributes with method usage only"
            );
    }

    private MethodDef GetConditionMethod(CustomAttribute attribute) {
        var targetType = ((ClassSig) attribute.ConstructorArguments[0].Value).TypeDef;
        var targetMethodName = ((UTF8String) attribute.ConstructorArguments[1].Value).String;
        var targetMethod = targetType.FindMethod(targetMethodName);
        AssertConditionMethodContract(targetMethod);
        return targetMethod;
    }

    private static void AssertConditionMethodContract(MethodDef method) {
        if (!method.HasReturnType || !method.ReturnType.IsPrimitive || method.ReturnType.FullName != "System.Boolean")
            throw new AttributeContractException($"Return type of \"{method}\" should be bool");
    }

    private void InjectPredicates(Dictionary<TypeDef, MethodDef> predicates) {
        foreach (var method in module.GetTypes().SelectMany(it => it.Methods)) {
            var attributes = method.CustomAttributes
                .Select(it => (type: it.AttributeType.ResolveTypeDef(), attribute: it))
                .Where(it => it.type != null && predicates.ContainsKey(it.type))
                .Select(it => (it.attribute, predicate: predicates[it.type]))
                .ToList();
            attributes.ForEach(it => PatchMethod(method, it.predicate, it.attribute));
        }
    }

    private void PatchMethod(MethodDef method, MethodDef condition, CustomAttribute attribute) {
        AssertTargetMethodContract(method);
        var arguments = ResolveArguments(condition, attribute);
        var source = method.Body.Instructions;
        var instructions = new List<Instruction>();
        var beginning = source.First().SequencePoint;

        arguments.ForEach(it => instructions.Add(GetLoadArgumentInstruction(it).WithSequencePoint(beginning)));
        instructions.Add(OpCodes.Call.ToInstruction(condition).WithSequencePoint(beginning));
        instructions.Add(OpCodes.Ldc_I4_0.ToInstruction().WithSequencePoint(beginning));
        instructions.Add(OpCodes.Ceq.ToInstruction().WithSequencePoint(beginning));
        instructions.Add(OpCodes.Brfalse_S.ToInstruction(source.First()).WithSequencePoint(beginning));

        Instruction? retInstruction = null;
        if (source.Last().OpCode.Code != Code.Ret)
            retInstruction = OpCodes.Ret.ToInstruction().WithSequencePoint(beginning);

        instructions.Add(OpCodes.Br_S.ToInstruction(retInstruction ?? source.Last()).WithSequencePoint(beginning));
        instructions.AddRange(source);
        if (retInstruction != null)
            instructions.Add(retInstruction);

        method.Body = new CilBody(
            method.Body.InitLocals,
            instructions,
            method.Body.ExceptionHandlers,
            method.Body.Variables
        );
    }

    private void AssertTargetMethodContract(MethodDef method) {
        if (method.HasReturnType)
            throw new AttributeContractException($"Non void return types aren't supported (method {method})");
    }

    private IEnumerable<object> ResolveArguments(MethodDef condition, CustomAttribute attribute) {
        var values = new Dictionary<(string, string), object>();

        foreach (var argument in attribute.NamedArguments)
            values[(argument.Name.String.Decapitalize()!, argument.Type.FullName)] = argument.Value;

        if (attribute.Constructor is MethodDef constructor)
            attribute.ConstructorArguments
                .Zip(constructor.Parameters.Skip(1), (argument, parameter) => (parameter, value: argument.Value))
                .ForEach(it => values[(it.parameter.Name.Decapitalize()!, it.parameter.Type.FullName)] = it.value);

        return condition.Parameters.Select(
            it => {
                if (!values.TryGetValue((it.Name, it.Type.FullName), out var value))
                    throw new AttributeContractException(
                        $"Unable to bind parameter \"{it.Name}\" of method \"{condition}\""
                    );

                return value;
            }
        );
    }

    private Instruction GetLoadArgumentInstruction(object value) => value switch {
        int intValue => OpCodes.Ldc_I4.ToInstruction(intValue),
        long longValue => OpCodes.Ldc_I8.ToInstruction(longValue),
        float floatValue => OpCodes.Ldc_I4.ToInstruction(floatValue),
        double doubleValue => OpCodes.Ldc_I8.ToInstruction(doubleValue),
        bool boolValue => (boolValue ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0).ToInstruction(),
        UTF8String stringValue => OpCodes.Ldstr.ToInstruction(stringValue.String),
        _ => throw new AttributeContractException($"Unsupported parameter type {value.GetType()}")
    };

}
