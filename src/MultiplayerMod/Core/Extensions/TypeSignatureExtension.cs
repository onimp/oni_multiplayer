using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiplayerMod.Core.Extensions;

[Flags]
public enum SignatureOptions {
    None = 0,
    IncludeNamespace = 1,
    IncludeInheritanceChain = 2,
    IncludeParametersName = 4
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
        return options.HasFlag(SignatureOptions.IncludeInheritanceChain)
            ? string.Join(" : ", GetTypeInheritanceChain(type).Select(it => GetTypeName(it, options)))
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

        var includeNamespace = (options & SignatureOptions.IncludeNamespace) != 0 && !type.IsNested;
        var name = includeNamespace ? $"{type.Namespace}.{type.Name}" : type.Name;

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

    public static IEnumerable<Type> GetTypeInheritanceChain(Type type) {
        var current = type;
        while (current != typeof(object) && current != null) {
            yield return current;

            current = current.BaseType;
        }
    }

}
