using System;
using System.Collections.Generic;
using System.Text;
using dnlib.DotNet;

namespace AssemblyExposer.MSBuild.Task;

public static class VisibilityExtensions {

    private static readonly Dictionary<IMemberDef, string> cache = new();

    public static void ApplyRule(this IMemberDef def, RewriteRule rule) {
        switch (def) {
            case FieldDef fieldDef:
                fieldDef.ApplyRule(rule);
                break;
            case MethodDef methodDef:
                methodDef.ApplyRule(rule);
                break;
            case TypeDef typeDef:
                typeDef.ApplyRule(rule);
                break;
        }
    }

    private static void ApplyRule(this TypeDef type, RewriteRule rule) {
        var visibility = rule.Visibility switch {
            VisibilityOption.Public => type.IsNested
                ? TypeAttributes.NestedPublic
                : TypeAttributes.Public,
            VisibilityOption.Protected => type.IsNested
                ? TypeAttributes.NestedFamily
                : type.Attributes & TypeAttributes.VisibilityMask,
            _ => throw new Exception("Invalid visibility value")
        };
        type.Attributes = (type.Attributes & ~TypeAttributes.VisibilityMask) | visibility;
    }

    private static void ApplyRule(this MethodDef method, RewriteRule rule) {
        var visibility = rule.Visibility switch {
            VisibilityOption.Public => MethodAttributes.Public,
            VisibilityOption.Protected => MethodAttributes.Family,
            _ => throw new Exception("Invalid visibility value")
        };
        method.Attributes = (method.Attributes & ~MethodAttributes.MemberAccessMask) | visibility;
    }

    private static void ApplyRule(this FieldDef field, RewriteRule rule) {
        var visibility = rule.Visibility switch {
            VisibilityOption.Public => FieldAttributes.Public,
            VisibilityOption.Protected => FieldAttributes.Family,
            _ => throw new Exception("Invalid visibility value")
        };
        field.Attributes = (field.Attributes & ~FieldAttributes.FieldAccessMask) | visibility;
    }

    public static string ExtendedName(this IMemberDef def) => def switch {
        TypeDef typeDef => typeDef.ExtendedName(),
        MethodDef methodDef => methodDef.ExtendedName(),
        FieldDef fieldDef => fieldDef.ExtendedName(),
        _ => throw new Exception("Unsupported definition")
    };

    private static string ExtendedName(this TypeDef def) {
        if (cache.TryGetValue(def, out var name))
            return name;

        StringBuilder builder = new StringBuilder();
        switch (def.Visibility) {
            case TypeAttributes.NotPublic:
            case TypeAttributes.NestedAssembly:
                builder.Append("internal ");
                break;
            case TypeAttributes.NestedFamily:
                builder.Append("protected ");
                break;
            case TypeAttributes.NestedFamORAssem:
                builder.Append("protected internal ");
                break;
            case TypeAttributes.Public:
            case TypeAttributes.NestedPublic:
                builder.Append("public ");
                break;
            case TypeAttributes.NestedPrivate:
                builder.Append("private ");
                break;
        }

        var isInterface = (def.Attributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.Interface;
        if (isInterface) {
            builder.Append("interface ");
        } else {
            var isSealed = def.Attributes.HasFlag(TypeAttributes.Sealed);
            if (def.Attributes.HasFlag(TypeAttributes.Abstract))
                builder.Append(isSealed ? "static " : "abstract ");
            else if (isSealed)
                builder.Append("sealed ");
            builder.Append("class ");
        }

        builder.Append(def.FullName);
        return cache[def] = builder.ToString();
    }

    private static string ExtendedName(this MethodDef def) {
        if (cache.TryGetValue(def, out var name))
            return name;

        StringBuilder builder = new StringBuilder();
        switch (def.Access) {
            case MethodAttributes.Private:
                builder.Append("private ");
                break;
            case MethodAttributes.Assembly:
                builder.Append("internal ");
                break;
            case MethodAttributes.Family:
                builder.Append("protected ");
                break;
            case MethodAttributes.Public:
                builder.Append("public ");
                break;
            case MethodAttributes.FamORAssem:
                builder.Append("protected internal ");
                break;
        }

        var isInterface = def.DeclaringType.Attributes.HasFlag(TypeAttributes.Interface);
        var isFinal = def.Attributes.HasFlag(MethodAttributes.Final);

        if (def.Attributes.HasFlag(MethodAttributes.Virtual) && !isInterface && !isFinal)
            builder.Append(def.Attributes.HasFlag(MethodAttributes.NewSlot) ? "virtual " : "override ");

        if (def.Attributes.HasFlag(MethodAttributes.Static))
            builder.Append("static ");

        if (def.Attributes.HasFlag(MethodAttributes.Abstract) && !isInterface)
            builder.Append("abstract ");

        builder.Append(def.FullName);
        return cache[def] = builder.ToString();
    }

    private static string ExtendedName(this FieldDef def) {
        if (cache.TryGetValue(def, out var name))
            return name;

        StringBuilder builder = new StringBuilder();
        switch (def.Access) {
            case FieldAttributes.Private:
                builder.Append("private ");
                break;
            case FieldAttributes.Assembly:
                builder.Append("internal ");
                break;
            case FieldAttributes.Family:
                builder.Append("protected ");
                break;
            case FieldAttributes.Public:
                builder.Append("public ");
                break;
            case FieldAttributes.FamORAssem:
                builder.Append("protected internal ");
                break;
        }

        if (def.Attributes.HasFlag(FieldAttributes.Static))
            builder.Append("static ");
        if (def.Attributes.HasFlag(FieldAttributes.InitOnly))
            builder.Append("readonly ");
        if (def.Attributes.HasFlag(FieldAttributes.Literal))
            builder.Append("const ");

        builder.Append(def.FullName);
        return cache[def] = builder.ToString();
    }

}
