using System;
using System.Collections.Generic;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Game.Context;
using MultiplayerMod.Game.UI.Tools.Context;
using MultiplayerMod.Game.UI.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class AbstractDragToolCommand<T> : IMultiplayerCommand where T : DragTool, new() {

    protected DragCompleteEventArgs Arguments;

    protected AbstractDragToolCommand(DragCompleteEventArgs arguments) {
        Arguments = arguments;
    }

    // ReSharper disable once Unity.IncorrectMonoBehaviourInstantiation
    public void Execute() {
        var tool = new T();
        InitializeTool(tool);
        GameContext.Override(CreateContext(), () => InvokeTool(tool));
    }

    protected virtual IGameContext CreateContext() => new PrioritySettingsContext(Arguments.Priority);

    protected virtual void InitializeTool(T tool) {
        tool.downPos = Arguments.CursorDown;

        if (tool is not FilteredDragTool filteredTool)
            return;

        filteredTool.currentFilterTargets = new Dictionary<string, ToolParameterMenu.ToggleState> {
            [ToolParameterMenu.FILTERLAYERS.ALL] = ToolParameterMenu.ToggleState.Off
        };
        Arguments.Parameters?.ForEach(it => filteredTool.currentFilterTargets[it] = ToolParameterMenu.ToggleState.On);
    }

    protected virtual void InvokeTool(T tool) => Arguments.Cells.ForEach(it => tool.OnDragTool(it, 0));

}
