using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiplayerMod.Core.Extensions;

[Flags]
public enum SignatureOptions {
    None = 0,
    Namespace = 1,
    Inheritance = 2,
    ParametersName = 4,
    NoDeclaringType = 8
}

public static class TypeSignatureExtension {

    private static readonly Dictionary<Type, string> builtInAliases = new() {
        { typeof(bool), "bool" },
        { typeof(byte), "byte" },
        { typeof(sbyte), "sbyte" },
        { typeof(char), "char" },
        { typeof(decimal), "decimal" },
        { typeof(double), "double" },
        { typeof(float), "float" },
        { typeof(int), "int" },
        { typeof(uint), "uint" },
        { typeof(nint), "nint" },
        { typeof(nuint), "nuint" },
        { typeof(long), "long" },
        { typeof(ulong), "ulong" },
        { typeof(short), "short" },
        { typeof(ushort), "ushort" },
        { typeof(object), "object" },
        { typeof(string), "string" },
        { typeof(void), "void" }
    };

    public static string GetSignature(this Type type, SignatureOptions options = SignatureOptions.None) {
        return options.HasFlag(SignatureOptions.Inheritance)
            ? string.Join(" : ", type.GetInheritedTypes().Select(it => GetTypeName(it, options)))
            : GetTypeName(type, options);
    }

    private static string GetTypeName(Type type, SignatureOptions options) => GetTypeName(
        type,
        options,
        type.GetGenericArguments(),
        0
    );

    private static string GetTypeName(Type type, SignatureOptions options, Type[] genericArguments, int endOffset) {
        if (builtInAliases.TryGetValue(type, out var alias))
            return alias;

        if (type == typeof(object))
            return "object";

        if (type.IsGenericParameter)
            return type.Name;

        if (type.IsArray)
            return $"{GetTypeName(type.GetElementType()!, options)}[]";

        if (type.IsByRef)
            return $"{GetTypeName(type.GetElementType()!, options)}&";

        var name = type.Name;
        if (options.HasFlag(SignatureOptions.Namespace) && !type.IsNested && !string.IsNullOrEmpty(type.Namespace))
            name = $"{type.Namespace}.{type.Name}";

        if (!type.IsGenericType) {
            return type.IsNested
                ? $"{GetTypeName(type.DeclaringType!, options, genericArguments, endOffset)}.{name}"
                : name;
        }

        var typeDefinition = type.GetGenericTypeDefinition();
        var genericParametersCount = typeDefinition.GetGenericArguments().Length;
        if (type.IsNested) {
            genericParametersCount -= typeDefinition.DeclaringType!.GetGenericArguments().Length;
            var declaringTypeName = GetTypeName(
                type.DeclaringType!,
                options,
                genericArguments,
                endOffset + genericParametersCount
            );
            name = $"{declaringTypeName}.{name}";
        }

        if (genericParametersCount == 0)
            return name;

        name = name.Substring(0, name.LastIndexOf("`", StringComparison.InvariantCulture));
        var offset = genericArguments.Length - (endOffset + genericParametersCount);
        var arguments = genericArguments.Skip(offset).Take(genericParametersCount);

        name = typeDefinition == typeof(Nullable<>)
            ? $"{GetTypeName(arguments.First(), options)}?"
            : $"{name}<{string.Join(", ", arguments.Select(it => GetTypeName(it, options)))}>";

        return name;
    }

}
