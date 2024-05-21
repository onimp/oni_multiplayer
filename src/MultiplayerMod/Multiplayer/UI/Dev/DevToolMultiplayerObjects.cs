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
        if (!ImGui.BeginTable("Multiplayer objects:", 5, flags))
            return;

        ImGui.TableSetupColumn("Id", ImGuiTableColumnFlags.WidthFixed);
        ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed);
        ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.WidthFixed);

        ImGui.TableHeadersRow();

        var index = 0;
        multiplayer.Objects.Where(it => !generatedOnly || generatedOnly && it.Key.ToString().EndsWith(":0")).ForEach(
            it => {
                ImGui.TableNextRow();
                ImGui.PushID($"ID_row_{index++}");
                switch (it.Value) {
                    case GameObject gameObject:
                        RenderGameObjectRow(it.Key, gameObject);
                        break;
                    case Chore chore:
                        RenderChoreRow(it.Key, chore);
                        break;
                    default:
                        RenderUnsupportedObjectRow(it.Key, it.Value);
                        break;
                }
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

    private void RenderUnsupportedObjectRow(MultiplayerId id, object instance) {
        ImGui.TableNextColumn();
        ImGui.Text(id.ToString());
        ImGui.TableNextColumn();
        ImGui.Text($"[Unsupported] {instance.GetType().Name}");
        ImGui.TableNextColumn();
        ImGui.Text("N/A");
        ImGui.TableNextColumn();
    }

    private void RenderChoreRow(MultiplayerId id, Chore chore) {
        ImGui.TableNextColumn();
        ImGui.Text(id.ToString());
        ImGui.TableNextColumn();
        ImGui.Text(nameof(Chore));
        ImGui.TableNextColumn();
        ImGui.Text(chore.choreType.Name);
        ImGui.TableNextColumn();
    }

    private void RenderGameObjectRow(MultiplayerId id, GameObject gameObject) {
        ImGui.TableNextColumn();
        ImGui.Text(id.ToString());
        ImGui.TableNextColumn();
        ImGui.Text(nameof(GameObject));
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
