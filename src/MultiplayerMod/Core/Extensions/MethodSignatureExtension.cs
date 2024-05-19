using System.Linq;
using System.Reflection;
using System.Text;

namespace MultiplayerMod.Core.Extensions;

public static class MethodSignatureExtension {

    public static string GetSignature(this MethodBase method, SignatureOptions options = SignatureOptions.None) {
        var result = new StringBuilder();
        if (!options.HasFlag(SignatureOptions.NoDeclaringType)) {
            result.Append(method.DeclaringType!.GetSignature());
            result.Append(method.IsStatic ? "." : ":");
        }
        result.Append(GetMethodSignature(method, options));
        return result.ToString();
    }

    private static string GetMethodSignature(MethodBase method, SignatureOptions options) {
        var result = new StringBuilder();
        var parameters = method.GetParameters().Select(it => GetParameterSignature(it, options));

        result.Append(method.Name);
        if (method.IsGenericMethod) {
            var arguments = method.GetGenericArguments()
                .Select(it => it.GetSignature(options));
            result
                .Append("<")
                .Append(string.Join(", ", arguments))
                .Append(">");
        }
        result
            .Append("(")
            .Append(string.Join(", ", parameters))
            .Append(")");
        return result.ToString();
    }

    private static string GetParameterSignature(ParameterInfo parameter, SignatureOptions options) {
        var result = new StringBuilder();
        result.Append(parameter.ParameterType.GetSignature(options));

        if (options.HasFlag(SignatureOptions.ParametersName))
            result.Append(" ").Append(parameter.Name);
        return result.ToString();
    }

}
