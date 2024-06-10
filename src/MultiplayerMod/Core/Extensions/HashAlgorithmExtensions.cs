using System;
using System.Reflection;
using System.Security.Cryptography;

namespace MultiplayerMod.Core.Extensions;

public static class HashAlgorithmExtensions {

    public static byte[] ComputeHash(this HashAlgorithm algorithm, MethodBase method) {
        var body = method.GetMethodBody() ?? throw new Exception(
            $"Unable to compute hash of " +
            $"{method.DeclaringType?.GetSignature(SignatureOptions.Namespace)}:{method.Name}, " +
            $"no method body available"
        );
        return algorithm.ComputeHash(body.GetILAsByteArray());
    }

}
