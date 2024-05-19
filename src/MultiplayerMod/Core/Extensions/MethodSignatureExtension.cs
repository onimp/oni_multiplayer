using System.Linq;
using System.Reflection;

namespace MultiplayerMod.Core.Extensions;

public static class MethodSignatureExtension {

    public static string GetSignature(this MethodBase method, SignatureOptions options = SignatureOptions.None) {
        var typeName = method.DeclaringType!.GetSignature();
        var separator = method.IsStatic ? "." : ":";
        return $"{typeName}{separator}{GetMethodSignature(method, options)}";
    }

    private static string GetMethodSignature(MethodBase method, SignatureOptions options) {
        var parameters = method.GetParameters().Select(it => GetParameterSignature(it, options));
        var genericSignature = "";
        if (method.IsGenericMethod) {
            var arguments = method.GetGenericArguments()
                .Select(it => it.GetSignature(options));
            genericSignature = $"<{string.Join(", ", arguments)}>";
        }
        return $"{method.Name}{genericSignature}({string.Join(", ", parameters)})";
    }

    private static string GetParameterSignature(ParameterInfo it, SignatureOptions options) {
        var typeSignature = it.ParameterType.GetSignature(options);
        var includeName = options.HasFlag(SignatureOptions.IncludeParametersName);
        return $"{typeSignature}{(includeName ? $" {it.Name}" : "")}";
    }

}
