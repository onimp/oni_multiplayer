using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Players;

namespace MultiplayerMod.Multiplayer.Ownership;

/// <summary>
/// Host-only persistence for duplicant ownership. The in-memory registry is keyed by a duplicant's proxy
/// <see cref="MultiplayerId"/> (stable across a host save/reload because it derives from the serialized
/// <c>KPrefabID.InstanceID</c>) but by a per-session <see cref="PlayerIdentity"/> that is NOT stable across a
/// reload. So we persist ownership as <c>proxyId -&gt; player NAME</c> in a small sidecar file next to the
/// <c>.sav</c>, and re-map names back to the current players on load.
///
/// This is deliberately host-local and outside ONI's binary save format: it is additive and self-healing.
/// If the sidecar is missing, unreadable, or a stored name no longer matches a connected player, those
/// duplicants simply fall through to the normal round-robin auto-split - i.e. exactly today's behaviour.
/// </summary>
[Dependency, UsedImplicitly]
public class DuplicantOwnershipPersistence {

    private const string FileSuffix = ".mpownership";
    private const string FormatHeader = "mp-ownership v1";

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<DuplicantOwnershipPersistence>();

    private readonly MultiplayerGame multiplayer;
    private readonly DuplicantOwnershipRegistry registry;

    public DuplicantOwnershipPersistence(
        EventDispatcher events,
        MultiplayerGame multiplayer,
        DuplicantOwnershipRegistry registry
    ) {
        this.multiplayer = multiplayer;
        this.registry = registry;
        events.Subscribe<WorldSavedEvent>(_ => Save());
    }

    /// <summary>The snapshot restored from the sidecar file, or null when there is nothing to restore.</summary>
    public sealed record PersistedOwnership(bool Enabled, IReadOnlyList<PersistedOwner> Owners);

    public sealed record PersistedOwner(MultiplayerId ProxyId, string PlayerName);

    /// <summary>Writes the current ownership map to the sidecar next to the active save. Never throws.</summary>
    private void Save() {
        if (multiplayer.Mode != MultiplayerMode.Host)
            return;

        var path = SidecarPath();
        if (path == null)
            return;

        try {
            var lines = new List<string> {
                FormatHeader,
                "enabled " + (registry.Enabled ? "1" : "0")
            };
            foreach (var pair in registry.Snapshot()) {
                var name = ResolvePlayerName(pair.Value);
                if (name == null)
                    continue; // owner no longer maps to a known player - let it auto-split on reload
                lines.Add(string.Join("\t", pair.Key.HighPart.ToString(CultureInfo.InvariantCulture),
                    pair.Key.LowPart.ToString(CultureInfo.InvariantCulture), Escape(name)));
            }
            File.WriteAllLines(path, lines);
            log.Debug($"Saved {lines.Count - 2} duplicant ownership entries to {path}");
        } catch (Exception e) {
            log.Warning($"Failed to persist duplicant ownership: {e.Message}");
        }
    }

    /// <summary>Reads the sidecar for the active save, or null if absent/unreadable. Never throws.</summary>
    public PersistedOwnership? Load() {
        var path = SidecarPath();
        if (path == null || !File.Exists(path))
            return null;

        try {
            var lines = File.ReadAllLines(path);
            if (lines.Length == 0 || lines[0] != FormatHeader)
                return null;

            var enabled = true;
            var owners = new List<PersistedOwner>();
            foreach (var line in lines.Skip(1)) {
                if (line.StartsWith("enabled ")) {
                    enabled = line.Substring("enabled ".Length).Trim() == "1";
                    continue;
                }
                var parts = line.Split(new[] { '\t' }, 3);
                if (parts.Length < 3)
                    continue;
                if (!long.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var high) ||
                    !long.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var low))
                    continue;
                owners.Add(new PersistedOwner(
                    new MultiplayerId((InternalMultiplayerIdType) high, low),
                    Unescape(parts[2])
                ));
            }
            log.Debug($"Loaded {owners.Count} duplicant ownership entries from {path}");
            return new PersistedOwnership(enabled, owners);
        } catch (Exception e) {
            log.Warning($"Failed to load persisted duplicant ownership: {e.Message}");
            return null;
        }
    }

    private string? ResolvePlayerName(PlayerIdentity owner) =>
        multiplayer.Players.FirstOrDefault(it => it.Id.Equals(owner))?.Profile.PlayerName;

    private static string? SidecarPath() {
        try {
            var save = SaveLoader.GetActiveSaveFilePath();
            return string.IsNullOrEmpty(save) ? null : save + FileSuffix;
        } catch {
            return null;
        }
    }

    // Guard the tab/newline delimiters against exotic player names without needing a real encoder.
    private static string Escape(string value) => value.Replace("\\", "\\\\").Replace("\t", "\\t").Replace("\n", "\\n");

    private static string Unescape(string value) => value.Replace("\\t", "\t").Replace("\\n", "\n").Replace("\\\\", "\\");

}
