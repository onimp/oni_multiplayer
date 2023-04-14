using System;
using System.Collections.Generic;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Game.Context;
using MultiplayerMod.Game.Events.Tools;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class AbstractDragToolCommand<T> : IMultiplayerCommand where T : DragTool, new() {

    protected DragCompleteEventArgs Event;

    protected AbstractDragToolCommand(DragCompleteEventArgs @event) {
        Event = @event;
    }

    // ReSharper disable once Unity.IncorrectMonoBehaviourInstantiation
    public void Execute() {
        var tool = new T();
        InitializeTool(tool);
        var context = new OverrideContext { Priority = Event.Priority };
        GameContextManager.Override(context, () => InvokeTool(tool));
    }

    protected virtual void InitializeTool(T tool) {
        tool.downPos = Event.CursorDown;

        if (tool is not FilteredDragTool filteredTool)
            return;

        filteredTool.currentFilterTargets = new Dictionary<string, ToolParameterMenu.ToggleState> {
            [ToolParameterMenu.FILTERLAYERS.ALL] = ToolParameterMenu.ToggleState.Off
        };
        Event.Parameters?.ForEach(it => filteredTool.currentFilterTargets[it] = ToolParameterMenu.ToggleState.On);
    }

    protected virtual void InvokeTool(T tool) => Event.Cells.ForEach(it => tool.OnDragTool(it, 0));

}
