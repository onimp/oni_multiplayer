using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiplayerMod.Core.Extensions;

[Flags]
public enum TypeNameOptions {
    None = 0,
    IncludeNamespace = 1,
    IncludeInheritanceChain = 2
}

public static class TypeNameExtension {

    public static string GetPrettyName(this Type type, TypeNameOptions options = TypeNameOptions.None) {
        return options.HasFlag(TypeNameOptions.IncludeInheritanceChain)
            ? string.Join(" : ", GetTypeInheritanceChain(type).Select(it => GetTypeName(it, options)))
            : GetTypeName(type, options);
    }

    private static string GetTypeName(Type type, TypeNameOptions options) {
        return GetTypeName(type, options, type.GetGenericArguments(), 0);
    }

    private static string GetTypeName(Type type, TypeNameOptions options, Type[] genericArguments, int endOffset) {
        if (type.IsArray)
            return $"{GetTypeName(type.GetElementType()!, options)}[]";

        var includeNamespace = (options & TypeNameOptions.IncludeNamespace) != 0 && !type.IsNested;
        var name = includeNamespace ? $"{type.Namespace}.{type.Name}" : type.Name;

        if (!type.IsGenericType)
            return name;

        var genericParametersCount = type.GetGenericTypeDefinition().GetGenericArguments().Length;
        if (type.IsNested) {
            genericParametersCount -= type.GetGenericTypeDefinition().DeclaringType!.GetGenericArguments().Length;
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
        name = $"{name}<{string.Join(", ", arguments.Select(it => GetTypeName(it, options)))}>";

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
