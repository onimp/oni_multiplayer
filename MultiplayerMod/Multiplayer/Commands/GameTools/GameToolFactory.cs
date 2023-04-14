using System;
using System.Diagnostics.CodeAnalysis;
using MultiplayerMod.Multiplayer.Patches;

namespace MultiplayerMod.Multiplayer.Commands.GameTools;

public static class GameToolFactory {

    [SuppressMessage("ReSharper", "Unity.IncorrectMonoBehaviourInstantiation")]
    public static Action<object> CreateToolAction(GameToolType type) {
        return type switch {
            GameToolType.Build => payload => DragToolEffect.OnDragTool(BuildTool.Instance, payload),
            GameToolType.UtilityBuild => payload => DragToolEffect.OnDragTool(UtilityBuildTool.Instance, payload),
            GameToolType.WireBuild => payload => DragToolEffect.OnDragTool(WireBuildTool.Instance, payload),
            GameToolType.CopySettings => payload => DragToolEffect.OnDragTool(CopySettingsTool.Instance, payload),
            GameToolType.Debug => payload => DragToolEffect.OnDragTool(DebugTool.Instance, payload),
            GameToolType.Place => payload => DragToolEffect.OnDragTool(PlaceTool.Instance, payload),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

}
