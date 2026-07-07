using System.Linq;
using ImGuiNET;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Game.Mechanics.Minions;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Ownership;
using MultiplayerMod.Multiplayer.Players;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.UI.Dev;

// Read-only inspector for the per-player duplicant ownership feature: every live duplicant, who owns it,
// and what task it's currently doing. The "Chore owner" column (host only) shows the owning player of the
// duplicant's current work target - useful for confirming the Phase 2 preference is doing what you expect.
public class DevToolDuplicantOwnership : DevTool {

    [InjectDependency]
    private readonly MultiplayerGame multiplayer = null!;

    [InjectDependency]
    private readonly DuplicantOwnershipRegistry ownership = null!;

    [InjectDependency]
    private readonly ChoreOwnershipRegistry choreOwnership = null!;

    [InjectDependency]
    private readonly ExecutionLevelManager executionLevelManager = null!;

    private static readonly Vector4 mineColor = new(0.4f, 0.85f, 0.4f, 1f);
    private static readonly Vector4 unownedColor = new(0.7f, 0.7f, 0.7f, 1f);

    public DevToolDuplicantOwnership() {
        Dependencies.Get<IDependencyInjector>().Inject(this);
        Name = "Duplicant Ownership";
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

        ImGui.Text($"Feature: {(ownership.Enabled ? "ENABLED" : "disabled")}   Mode: {multiplayer.Mode}");
        ImGui.Spacing();

        const ImGuiTableFlags flags = ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg |
                                      ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollX |
                                      ImGuiTableFlags.ScrollY | ImGuiTableFlags.Resizable;
        if (!ImGui.BeginTable("Duplicant ownership:", 5, flags))
            return;

        ImGui.TableSetupColumn("Duplicant", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn("Proxy id", ImGuiTableColumnFlags.WidthFixed);
        ImGui.TableSetupColumn("Owner", ImGuiTableColumnFlags.WidthFixed);
        ImGui.TableSetupColumn("Task", ImGuiTableColumnFlags.WidthFixed);
        ImGui.TableSetupColumn("Chore owner (host)", ImGuiTableColumnFlags.WidthFixed);
        ImGui.TableHeadersRow();

        var index = 0;
        foreach (var identity in global::Components.LiveMinionIdentities.Items) {
            if (identity == null)
                continue;
            ImGui.TableNextRow();
            ImGui.PushID($"own_row_{index++}");
            RenderRow(identity);
            ImGui.PopID();
        }

        ImGui.EndTable();
    }

    private void RenderRow(MinionIdentity identity) {
        var proxyId = TryGetProxyId(identity);
        var owner = proxyId == null ? null : ownership.GetOwner(proxyId);

        var gameObject = identity.gameObject;
        var chore = gameObject.GetComponent<ChoreDriver>()?.GetCurrentChore();
        var task = chore?.choreType?.Name ?? "-";
        var choreOwner = multiplayer.Mode == MultiplayerMode.Host && chore != null
            ? choreOwnership.GetOwner(chore.gameObject)
            : null;

        ImGui.TableNextColumn();
        ImGui.Text(identity.GetProperName());

        ImGui.TableNextColumn();
        ImGui.Text(proxyId?.ToString() ?? "-");

        ImGui.TableNextColumn();
        ImGui.TextColored(owner == null ? unownedColor : mineColor, PlayerName(owner));

        ImGui.TableNextColumn();
        ImGui.Text(task);

        ImGui.TableNextColumn();
        if (multiplayer.Mode != MultiplayerMode.Host)
            ImGui.TextColored(unownedColor, "host only");
        else
            ImGui.TextColored(choreOwner == null ? unownedColor : mineColor, PlayerName(choreOwner));
    }

    private string PlayerName(PlayerIdentity? id) {
        if (id == null)
            return "Unassigned";
        var player = multiplayer.Players.FirstOrDefault(it => it.Id.Equals(id));
        return player?.Profile.PlayerName ?? id.Value.ToString().Substring(0, 8);
    }

    private static MultiplayerId? TryGetProxyId(MinionIdentity identity) {
        try {
            return identity.GetMultiplayerInstance().Id;
        } catch {
            return null;
        }
    }

}
