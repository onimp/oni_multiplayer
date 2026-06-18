using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;

namespace MultiplayerMod.Multiplayer.Compatibility;

[Dependency, UsedImplicitly]
public class CompatibilityValidator {

    private static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;

    public CompatibilityValidationResult Validate(
        CompatibilityFingerprint host,
        CompatibilityFingerprint client
    ) {
        var mismatches = new List<CompatibilityMismatch>();

        if (host.ProtocolVersion != client.ProtocolVersion) {
            mismatches.Add(new CompatibilityMismatch(
                nameof(CompatibilityFingerprint.ProtocolVersion),
                $"Multiplayer protocol mismatch: host {host.ProtocolVersion}, client {client.ProtocolVersion}"
            ));
        }

        AddIfDifferent(mismatches, nameof(CompatibilityFingerprint.ModVersion), host.ModVersion, client.ModVersion);
        AddIfDifferent(mismatches, nameof(CompatibilityFingerprint.GameBuild), host.GameBuild, client.GameBuild);
        AddIfDifferent(mismatches, nameof(CompatibilityFingerprint.GameBranch), host.GameBranch, client.GameBranch);
        AddSetMismatch(
            mismatches,
            nameof(CompatibilityFingerprint.LoadedDlcIds),
            "Loaded DLC/content IDs differ",
            host.LoadedDlcIds,
            client.LoadedDlcIds,
            allowEmptyClient: false
        );
        AddSetMismatch(
            mismatches,
            nameof(CompatibilityFingerprint.ActiveDlcIds),
            "Active save DLC/content IDs differ",
            host.ActiveDlcIds,
            client.ActiveDlcIds,
            allowEmptyClient: true
        );
        AddModMismatch(mismatches, host.ActiveMods, client.ActiveMods);

        return new CompatibilityValidationResult(mismatches.ToArray());
    }

    private static void AddIfDifferent(
        List<CompatibilityMismatch> mismatches,
        string field,
        string host,
        string client
    ) {
        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(client))
            return;

        if (!Comparer.Equals(host, client)) {
            mismatches.Add(new CompatibilityMismatch(
                field,
                $"{field} mismatch: host '{host}', client '{client}'"
            ));
        }
    }

    private static void AddSetMismatch(
        List<CompatibilityMismatch> mismatches,
        string field,
        string message,
        IEnumerable<string> host,
        IEnumerable<string> client,
        bool allowEmptyClient
    ) {
        var hostValues = Normalize(host);
        var clientValues = Normalize(client);
        if (hostValues.Length == 0 && clientValues.Length == 0)
            return;
        if (allowEmptyClient && clientValues.Length == 0)
            return;
        if (hostValues.SequenceEqual(clientValues, Comparer))
            return;

        mismatches.Add(new CompatibilityMismatch(
            field,
            $"{message}: host [{string.Join(", ", hostValues)}], client [{string.Join(", ", clientValues)}]"
        ));
    }

    private static void AddModMismatch(
        List<CompatibilityMismatch> mismatches,
        IEnumerable<ModFingerprint> host,
        IEnumerable<ModFingerprint> client
    ) {
        var hostMods = NormalizeMods(host);
        var clientMods = NormalizeMods(client);
        if (hostMods.SequenceEqual(clientMods))
            return;

        mismatches.Add(new CompatibilityMismatch(
            nameof(CompatibilityFingerprint.ActiveMods),
            "Active mod list differs between host and client"
        ));
    }

    private static string[] Normalize(IEnumerable<string> values) => values
        .Where(it => !string.IsNullOrWhiteSpace(it))
        .Select(it => it.Trim())
        .Distinct(Comparer)
        .OrderBy(it => it, Comparer)
        .ToArray();

    private static string[] NormalizeMods(IEnumerable<ModFingerprint> mods) => mods
        .Where(it => !string.IsNullOrWhiteSpace(it.StaticId))
        .Select(it => $"{it.StaticId.Trim()}@{(it.Version ?? "").Trim()}")
        .Distinct(Comparer)
        .OrderBy(it => it, Comparer)
        .ToArray();

}
