using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Network;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.UI.Overlays;

/// <summary>
/// Builds the multi-line hard-sync progress text shown on every player's screen: a header with the
/// ready count and one line per player with their current phase and a live elapsed timer.
///
/// <see cref="Render"/> is called every frame by the overlay ticker, so the timer animates. Elapsed time
/// is measured locally per machine (reset whenever a player's phase changes) — never derived from a
/// remote timestamp — so there is no cross-machine clock skew.
/// </summary>
public class HardSyncStatusView {

    private readonly MultiplayerPlayers players;
    private readonly string title;

    // Live link stats for this machine's own connection (host: aggregate uplink to clients; client:
    // downlink from host) and the label to show for it. Null supplier => no network line.
    private readonly Func<ConnectionStats?>? linkStats;
    private readonly string linkLabel;

    // Per-player: the last phase-key we observed and the local time we first saw it (for the elapsed timer).
    private readonly Dictionary<PlayerIdentity, (string key, float since)> timers = new();

    public HardSyncStatusView(
        MultiplayerPlayers players,
        string title,
        Func<ConnectionStats?>? linkStats = null,
        string linkLabel = "Link"
    ) {
        this.players = players;
        this.title = title;
        this.linkStats = linkStats;
        this.linkLabel = linkLabel;
    }

    public string Render() {
        var now = Time.realtimeSinceStartup;
        var ordered = players.OrderByDescending(it => it.Role == PlayerRole.Host)
            .ThenBy(it => it.Profile.PlayerName)
            .ToList();
        var readyCount = ordered.Count(it => it.State == PlayerState.Ready);

        var builder = new StringBuilder();
        builder.AppendLine($"{title} — {readyCount}/{ordered.Count} ready");

        var stats = linkStats?.Invoke();
        if (stats.HasValue)
            builder.AppendLine(FormatLink(stats.Value));

        builder.AppendLine();

        foreach (var player in ordered) {
            var ready = player.State == PlayerState.Ready;
            var key = ready ? "ready" : player.LoadPhase.ToString();
            if (!timers.TryGetValue(player.Id, out var timer) || timer.key != key)
                timers[player.Id] = timer = (key, now);

            var name = player.Profile.PlayerName;
            if (ready)
                builder.AppendLine($"  {name}: Done");
            else
                builder.AppendLine($"  {name}: {PhaseLabel(player.LoadPhase)} ({(int) (now - timer.since)}s)");
        }

        return builder.ToString();
    }

    // e.g. "Uplink: ↑ 240.0 KB/s  ↓ 4.1 KB/s  ping 45ms  q 100%  queued 512 KiB"
    private string FormatLink(ConnectionStats stats) {
        var line = $"{linkLabel}: ↑ {Rate(stats.OutBytesPerSec)}  ↓ {Rate(stats.InBytesPerSec)}" +
                   $"  ping {stats.PingMs}ms  q {(int) (stats.ConnectionQuality * 100)}%";
        if (stats.PendingReliableBytes > 0)
            line += $"  queued {stats.PendingReliableBytes / 1024} KiB";
        return line;
    }

    private static string Rate(float bytesPerSec) => $"{bytesPerSec / 1024f:0.0} KB/s";

    private static string PhaseLabel(PlayerLoadPhase phase) => phase switch {
        PlayerLoadPhase.Preparing => "Preparing save",
        PlayerLoadPhase.Transferring => "Receiving world",
        PlayerLoadPhase.Loading => "Loading world",
        PlayerLoadPhase.Reconciling => "Reconciling state",
        _ => "Waiting"
    };

}
