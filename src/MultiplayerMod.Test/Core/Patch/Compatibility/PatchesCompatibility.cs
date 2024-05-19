using System;
using System.Collections.Generic;
using System.Linq;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Multiplayer.Chores;
using MultiplayerMod.Multiplayer.StateMachines.Configuration;
using NUnit.Framework;
using static MultiplayerMod.Test.Core.Patch.Compatibility.PatchesCompatibilityMetadata;

namespace MultiplayerMod.Test.Core.Patch.Compatibility;

[TestFixture]
public class PatchesCompatibility : AbstractGameTest {

    [Test]
    [Ignore("Manual run only")]
    public void Generate() {
        var methods = GetChoresStateMachines();
        var metadata = string.Join(",\n", methods);
        Console.WriteLine(metadata);
    }

    private static List<string> GetChoresStateMachines() {
        var context = new StateMachineConfigurationContext();
        ChoresMultiplayerConfiguration.Configuration
            .Select(it => it.StatesConfigurer)
            .NotNull()
            .ForEach(it => it.Configure(context));

        return context.Configurations
            .Select(it => it.StateMachineType)
            .Select(it => it.GetMethod(nameof(StateMachine.InitializeStates))!)
            .Select(
                it => (
                    method: it,
                    signature: it.GetSignature(SignatureOptions.NoDeclaringType),
                    hash: HashAlgorithm.ComputeHash(it).ToHexString()
                )
            )
            .Select(
                it =>
                    $"new(" +
                    $"typeof({it.method.DeclaringType!.GetSignature(SignatureOptions.Namespace)}), " +
                    $"\"{it.signature}\", " +
                    $"\"{it.hash}\"" +
                    $")"
            )
            .ToList();
    }

    [Test]
    public void Verify() {
        var possiblyIncompatible = new List<string>();
        foreach (var type in Metadata.Keys) {
            var methods = type.GetAllMethods()
                .Select(it => it.GetSignature(SignatureOptions.NoDeclaringType))
                .ToHashSet();
            var diff = Metadata[type]
                .Where(it => !methods.Contains(it.Key))
                .Select(it => $"{type.GetSignature(SignatureOptions.Namespace)}->{it.Value.Signature}");
            possiblyIncompatible.AddRange(diff);
        }
        if (possiblyIncompatible.Count > 0)
            Assert.Fail($"Possibly incompatible original methods found:\n{string.Join("\n", possiblyIncompatible)}\n");
    }

}
