// Record type support for .NET < 5

// ReSharper disable once CheckNamespace
// ReSharper disable once ArrangeNamespaceBody

namespace System.Runtime.CompilerServices;

// ReSharper disable once UnusedType.Global
internal static class IsExternalInit { }

// Caller argument expression support for .NET < 5
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class CallerArgumentExpressionAttribute : Attribute {

    public CallerArgumentExpressionAttribute(string parameterName) {
        ParameterName = parameterName;
    }

    public string ParameterName { get; }

}
