using System;
using System.Collections.Generic;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Game.Events.Tools;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Harvest : AbstractDragToolCommand<HarvestTool> {

    public Harvest(DragCompleteEventArgs @event) : base(@event) { }

    protected override void InitializeTool(HarvestTool tool) {
        base.InitializeTool(tool);
        tool.options = new Dictionary<string, ToolParameterMenu.ToggleState> {
            ["HARVEST_WHEN_READY"] = ToolParameterMenu.ToggleState.Off,
            ["DO_NOT_HARVEST"] = ToolParameterMenu.ToggleState.Off
        };
        Event.Parameters?.ForEach(it => tool.options[it] = ToolParameterMenu.ToggleState.On);
    }

}
