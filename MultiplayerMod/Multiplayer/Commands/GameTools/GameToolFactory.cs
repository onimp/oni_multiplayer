using System;
using System.Diagnostics.CodeAnalysis;
using MultiplayerMod.Multiplayer.Patches;

namespace MultiplayerMod.Multiplayer.Commands.GameTools;

public static class GameToolFactory {

    [SuppressMessage("ReSharper", "Unity.IncorrectMonoBehaviourInstantiation")]
    public static Action<object> CreateToolAction(GameToolType type) {
        return type switch {
            GameToolType.Dig => payload => DragToolEffect.OnDragTool(DigTool.Instance, payload),
            GameToolType.Cancel => payload => DragToolEffect.OnDragTool(CancelTool.Instance, payload),
            GameToolType.Deconstruct => payload => DragToolEffect.OnDragTool(DeconstructTool.Instance, payload),
            GameToolType.Prioritize => payload => DragToolEffect.OnDragTool(PrioritizeTool.Instance, payload),
            GameToolType.Disinfect => payload => DragToolEffect.OnDragTool(DisconnectTool.Instance, payload),
            GameToolType.Sweep => payload => DragToolEffect.OnDragTool(ClearTool.Instance, payload),
            GameToolType.Attack => payload => DragToolEffect.OnDragComplete(new AttackTool(), payload),
            GameToolType.Mop => payload => DragToolEffect.OnDragTool(MopTool.Instance, payload),
            GameToolType.Wrangle => payload => DragToolEffect.OnDragComplete(new CaptureTool(), payload),
            GameToolType.Harvest => payload => DragToolEffect.OnDragTool(HarvestTool.Instance, payload),
            GameToolType.EmptyPipe => payload => DragToolEffect.OnDragTool(EmptyPipeTool.Instance, payload),
            GameToolType.Disconnect => payload => DragToolEffect.OnDragTool(DisconnectTool.Instance, payload),
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
