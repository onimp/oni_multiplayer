using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;

namespace MultiplayerMod.Test.Core.Patch.Compatibility;

public static class PatchesCompatibilityMetadata {

    /// <summary>
    /// Generated metadata, see <see cref="PatchesCompatibility.Generate"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident")]
    private static readonly List<MethodMetadata> methodsMetadata = [

    ];

    public static readonly HashAlgorithm HashAlgorithm = SHA1.Create();

    public static readonly Dictionary<Type, Dictionary<string, MethodMetadata>> Metadata;

    static PatchesCompatibilityMetadata() {
        Metadata = methodsMetadata.GroupBy(it => it.Type)
            .ToDictionary(
                it => it.Key,
                grouping => grouping.ToDictionary(it => it.Signature, it => it)
            );
    }

    public record MethodMetadata(Type Type, string Signature, string Hash);

}
