using ImGuiNET;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer.World;

namespace MultiplayerMod.Multiplayer.UI.Dev;

// Host controls panel. Currently a single action: force the same save + world hard-sync that normally
// runs at the end of each day, on demand. Useful to pull a drifted client back into alignment (gas,
// buildings, etc.) without waiting for the daily cycle.
public class DevToolMultiplayerControls : DevTool {

    [InjectDependency]
    private readonly MultiplayerGame multiplayer = null!;

    [InjectDependency]
    private readonly ExecutionLevelManager executionLevelManager = null!;

    [InjectDependency]
    private readonly WorldManager worldManager = null!;

    public DevToolMultiplayerControls() {
        Dependencies.Get<IDependencyInjector>().Inject(this);
        Name = "Multiplayer Controls";
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
        if (multiplayer.Mode != MultiplayerMode.Host) {
            ImGui.Text("Only the host can force a sync.");
            return;
        }

        ImGui.Text("Save the game and push a full world hard-sync to all clients");
        ImGui.Text("(same operation as the end-of-day sync).");
        if (ImGui.Button("Force Save + Sync"))
            worldManager.Sync();
    }

}
