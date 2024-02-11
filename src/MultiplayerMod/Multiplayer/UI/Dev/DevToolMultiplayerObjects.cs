using System.Linq;
using ImGuiNET;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer.Objects;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.UI.Dev;

public class DevToolMultiplayerObjects : DevTool {

    [InjectDependency]
    private readonly MultiplayerGame multiplayer = null!;

    [InjectDependency]
    private readonly ExecutionLevelManager executionLevelManager = null!;

    private bool generatedOnly = true;
    private GameObject? lastSelected;

    public DevToolMultiplayerObjects() {
        Dependencies.Get<IDependencyInjector>().Inject(this);
        Name = "Multiplayer Objects";
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
        Render();
    }

    private void Render() {
        ImGui.Text($"Multiplayer is running in {multiplayer.Mode.ToString().ToLower()} mode");
        ImGui.Checkbox("Generated Only", ref generatedOnly);

        if (lastSelected != null) {
            ImGui.SameLine();
            if (ImGui.Button("Deselect"))
                lastSelected = null;
        }

        const ImGuiTableFlags flags = ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.SizingFixedFit |
                                      ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY | ImGuiTableFlags.Resizable;
        if (!ImGui.BeginTable("Multiplayer objects:", 4, flags))
            return;

        ImGui.TableSetupColumn("Instance Id", ImGuiTableColumnFlags.WidthFixed);
        ImGui.TableSetupColumn("UUID", ImGuiTableColumnFlags.WidthFixed);
        ImGui.TableSetupColumn("Game Object", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.WidthFixed);

        ImGui.TableHeadersRow();

        var index = 0;
        multiplayer.Objects.Where(it => !generatedOnly || generatedOnly && it.Key.InstanceId == 0).ForEach(
            it => {
                ImGui.TableNextRow();
                ImGui.PushID($"ID_row_{index++}");
                if (it.Value is GameObject gameObject)
                    RenderRow(it.Key, gameObject);
                ImGui.PopID();
            }
        );

        ImGui.EndTable();

        RenderSelectionOverlay();
    }

    private void RenderSelectionOverlay() {
        if (lastSelected == null)
            return;

        var target = new DevToolEntityTarget.ForWorldGameObject(lastSelected);
        Option<(Vector2, Vector2)> screenRect = target.GetScreenRect();
        if (screenRect.IsSome())
            DevToolEntity.DrawBoundingBox(screenRect.Unwrap(), target.GetDebugName(), ImGui.IsWindowFocused());
    }

    private void RenderRow(MultiplayerId id, GameObject gameObject) {
        ImGui.TableNextColumn();
        ImGui.Text(id.InstanceId.ToString());
        ImGui.TableNextColumn();
        ImGui.Text($"{id.UuidA:x16}{id.UuidB:x16}");
        ImGui.TableNextColumn();
        ImGui.Text(gameObject.GetProperName());
        ImGui.TableNextColumn();
        var selectable = !gameObject.GetComponent<KSelectable>().IsNullOrDestroyed();
        if (selectable) {
            if (ImGui.Button("Select")) {
                lastSelected = gameObject;
                SelectTool.Instance.Select(gameObject.GetComponent<KSelectable>());
            }
            ImGui.SameLine();
        }
        if (ImGui.Button("Inspect"))
            DevToolSceneInspector.Inspect(gameObject);
    }

}
