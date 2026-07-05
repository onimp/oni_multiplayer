using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace MultiplayerMod.Platform.Steam.Network.Messaging;

/// <summary>
/// Restricts <see cref="System.Runtime.Serialization.Formatters.Binary.BinaryFormatter"/>
/// deserialization of untrusted peer data to the types the multiplayer protocol legitimately sends,
/// blocking the classic BinaryFormatter gadget-chain remote-code-execution surface.
///
/// Every legitimate payload type lives in the mod, ONI game, or Unity assemblies; a small curated set
/// of framework primitives / collections / reflection holders is also required. In particular the
/// reflection holders are needed because <c>ArgumentUtils.DelegateRef</c> carries a
/// <see cref="MethodInfo"/> + <see cref="Type"/> over the wire — those deserialize to inert objects and
/// are semantically validated in <c>DelegateRef.Resolve</c> before they can ever be invoked. The
/// notorious <c>System.DelegateSerializationHolder</c> (native delegate deserialization, the
/// TypeConfuseDelegate gadget) is deliberately absent from the allowlist, so it is rejected.
/// </summary>
public class NetworkMessageSerializationBinder : SerializationBinder {

    public static readonly NetworkMessageSerializationBinder Instance = new();

    // Assemblies whose types are legitimately serialized: the decompiled game and Unity. First-party
    // assemblies (anything named "MultiplayerMod*", incl. the test assembly) are matched by prefix in
    // IsFirstPartyAssembly. A payload assembly name only admits types from an assembly actually loaded in
    // this process, so an attacker cannot invent a matching name.
    private static readonly HashSet<string> allowedAssemblies = new() {
        "Assembly-CSharp",
        "Assembly-CSharp-firstpass",
        "UnityEngine",
        "UnityEngine.CoreModule"
    };

    // Framework namespaces whose container types are inert and legitimately appear in payloads
    // (e.g. List&lt;Tag&gt;, HashSet&lt;Tag&gt;, object[]). Their element/argument types are checked recursively.
    private static readonly HashSet<string> allowedFrameworkNamespaces = new() {
        "System.Collections.Generic",
        "System.Collections"
    };

    // Individual framework types required by the protocol but not covered by the rules above.
    // Kept explicit and minimal — notably this list does NOT contain System.DelegateSerializationHolder.
    private static readonly HashSet<string> allowedFrameworkTypes = new() {
        // Reflection holders for DelegateRef's Type + MethodInfo fields (inert until DelegateRef.Resolve
        // validates and invokes them — see MultiplayerMod.Multiplayer.Commands.ArgumentUtils).
        "System.UnitySerializationHolder",
        "System.Reflection.MemberInfoSerializationHolder",
        "System.RuntimeType",
        "System.Reflection.RuntimeMethodInfo"
    };

    // Never allowed, even from an otherwise-allowed assembly (defence in depth).
    private static readonly string[] deniedNamespacePrefixes = {
        "System.Diagnostics",
        "System.IO",
        "System.Net",
        "System.Configuration",
        "System.Data",
        "System.Management",
        "System.Windows",
        "System.Workflow",
        "System.Activities",
        "System.Security",
        "System.CodeDom",
        "System.Runtime.Remoting"
    };

    public override Type? BindToType(string assemblyName, string typeName) {
        var type = ResolveType(assemblyName, typeName);
        if (type == null)
            throw new SerializationException($"Refusing to deserialize unresolvable type '{typeName}, {assemblyName}'");

        if (!IsAllowed(type))
            throw new SerializationException(
                $"Refusing to deserialize disallowed type '{type.FullName}' (possible gadget-chain RCE)"
            );

        return type;
    }

    /// <summary>
    /// Whether a type is trusted to appear in command data — the same allowlist that gates
    /// deserialization. Reused by <c>ArgumentUtils.DelegateRef</c> to refuse reconstructing a delegate
    /// onto a method declared by an untrusted type (the second half of the RCE mitigation).
    /// </summary>
    public static bool IsAllowedType(Type type) => IsAllowed(type);

    private static bool IsAllowed(Type type) {
        // Unwrap arrays and constructed generics; every leaf/argument must be allowed on its own.
        while (type.IsArray)
            type = type.GetElementType()!;

        if (type.IsConstructedGenericType) {
            foreach (var argument in type.GetGenericArguments())
                if (!IsAllowed(argument))
                    return false;
            type = type.GetGenericTypeDefinition();
        }

        if (type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(object) ||
            type == typeof(decimal) || type == typeof(DateTime) || type == typeof(TimeSpan) ||
            type == typeof(Guid))
            return IsAllowedNamespace(type);

        var assembly = type.Assembly.GetName().Name;
        if (assembly != null && (allowedAssemblies.Contains(assembly) || IsFirstPartyAssembly(assembly)))
            return IsAllowedNamespace(type);

        var ns = type.Namespace ?? "";
        return allowedFrameworkNamespaces.Contains(ns) || allowedFrameworkTypes.Contains(type.FullName ?? "");
    }

    private static bool IsFirstPartyAssembly(string assembly) =>
        assembly.StartsWith("MultiplayerMod", StringComparison.Ordinal);

    private static bool IsAllowedNamespace(Type type) {
        var ns = type.Namespace ?? "";
        foreach (var denied in deniedNamespacePrefixes)
            if (ns == denied || ns.StartsWith(denied + ".", StringComparison.Ordinal))
                return false;
        return true;
    }

    private static Type? ResolveType(string assemblyName, string typeName) {
        var type = Type.GetType(Assembly.CreateQualifiedName(assemblyName, typeName), throwOnError: false);
        if (type != null)
            return type;

        // Fallback: match by simple assembly name among loaded assemblies (tolerates version/culture drift).
        var simpleName = SafeSimpleName(assemblyName);
        if (simpleName == null)
            return null;

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            if (assembly.GetName().Name != simpleName)
                continue;
            type = assembly.GetType(typeName, throwOnError: false);
            if (type != null)
                return type;
        }
        return null;
    }

    private static string? SafeSimpleName(string assemblyName) {
        try {
            return new AssemblyName(assemblyName).Name;
        } catch (Exception) {
            return null;
        }
    }

}
