using System.Linq;
using ImGuiNET;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer.World.Debug;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.UI.Dev;

// Client-side inspector that joins the local duplicant state against the latest host snapshot
// (broadcast via SyncWorldDebugSnapshot) and flags per-field drift. This is the "can I tell if a
// duplicant is synced?" tool: green = matches host, red = drifted.
public class DevToolDuplicantSync : DevTool {

    [InjectDependency]
    private readonly MultiplayerGame multiplayer = null!;

    [InjectDependency]
    private readonly ExecutionLevelManager executionLevelManager = null!;

    private static readonly Vector4 okColor = new(0.4f, 0.85f, 0.4f, 1f);
    private static readonly Vector4 driftColor = new(0.95f, 0.4f, 0.4f, 1f);

    public DevToolDuplicantSync() {
        Dependencies.Get<IDependencyInjector>().Inject(this);
        Name = "Duplicant Sync";
    }

    protected override void RenderTo(DevPanel panel) {
        if (global::Game.Instance == null) {
            ImGui.Text("Game isn't running");
            return;
        }
        if (!executionLevelManager.LevelIsActive(ExecutionLevel.Multiplayer)) {
            ImGui.Text("Multiplayer isn't active");
            return;
        }

        if (multiplayer.Mode == MultiplayerMode.Host) {
            ImGui.Text("Host is authoritative — no peer to compare against.");
            return;
        }

        Render();
    }

    private void Render() {
        var host = WorldDebugSnapshotRunner.LastHostSnapshot;
        if (host == null) {
            ImGui.Text("Waiting for the first host snapshot (sent every 30s)...");
            return;
        }

        var hostById = host.Duplicants
            .Where(it => it.Id != null)
            .GroupBy(it => it.Id!.ToString())
            .ToDictionary(group => group.Key, group => group.First());

        var local = WorldDebugSnapshot.CreateDuplicantSnapshots();
        var inSync = local.Count(it => it.Id != null
                                       && hostById.TryGetValue(it.Id.ToString(), out var h)
                                       && Matches(it, h));

        ImGui.Text($"In sync: {inSync}/{local.Length}   (host time {host.WorldTime:0.0})");

        const ImGuiTableFlags flags = ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg |
                                      ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollX |
                                      ImGuiTableFlags.ScrollY | ImGuiTableFlags.Resizable;
        if (!ImGui.BeginTable("Duplicant sync:", 6, flags))
            return;

        ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn("Cell", ImGuiTableColumnFlags.WidthFixed);
        ImGui.TableSetupColumn("Chore", ImGuiTableColumnFlags.WidthFixed);
        ImGui.TableSetupColumn("State", ImGuiTableColumnFlags.WidthFixed);
        ImGui.TableSetupColumn("Nav target", ImGuiTableColumnFlags.WidthFixed);
        ImGui.TableSetupColumn("Sync", ImGuiTableColumnFlags.WidthFixed);
        ImGui.TableHeadersRow();

        var index = 0;
        foreach (var duplicant in local) {
            ImGui.TableNextRow();
            ImGui.PushID($"dupe_row_{index++}");
            DuplicantSnapshot? hostDuplicant = null;
            if (duplicant.Id != null && hostById.TryGetValue(duplicant.Id.ToString(), out var found))
                hostDuplicant = found;
            RenderRow(duplicant, hostDuplicant);
            ImGui.PopID();
        }

        ImGui.EndTable();
    }

    private void RenderRow(
        DuplicantSnapshot local,
        DuplicantSnapshot? host
    ) {
        ImGui.TableNextColumn();
        ImGui.Text(local.Name);

        RenderField(local.Cell.ToString(), host?.Cell.ToString(), host != null && local.Cell == host.Cell);
        RenderField(local.ChoreType, host?.ChoreType, host != null && local.ChoreType == host.ChoreType);
        RenderField(local.ChoreState, host?.ChoreState, host != null && local.ChoreState == host.ChoreState);
        RenderField(
            local.NavTargetCell.ToString(),
            host?.NavTargetCell.ToString(),
            host != null && local.NavTargetCell == host.NavTargetCell
        );

        ImGui.TableNextColumn();
        if (host == null)
            ImGui.TextColored(driftColor, "no host");
        else if (Matches(local, host))
            ImGui.TextColored(okColor, "ok");
        else
            ImGui.TextColored(driftColor, "drift");
    }

    private void RenderField(string localValue, string? hostValue, bool match) {
        ImGui.TableNextColumn();
        if (hostValue == null) {
            ImGui.Text(localValue);
            return;
        }
        if (match) {
            ImGui.Text(localValue);
            return;
        }
        ImGui.TextColored(driftColor, $"{localValue} (host: {hostValue})");
    }

    private static bool Matches(
        DuplicantSnapshot local,
        DuplicantSnapshot host
    ) {
        return local.Cell == host.Cell
               && local.ChoreType == host.ChoreType
               && local.ChoreState == host.ChoreState
               && local.NavTargetCell == host.NavTargetCell;
    }

}
