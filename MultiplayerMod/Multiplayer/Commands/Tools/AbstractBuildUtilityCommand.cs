using System;
using MultiplayerMod.Game.Context;
using MultiplayerMod.Game.Events.Tools;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class AbstractBuildUtilityCommand<T> : IMultiplayerCommand where T : BaseUtilityBuildTool, new() {

    protected UtilityBuildEventArgs Event;

    public AbstractBuildUtilityCommand(UtilityBuildEventArgs @event) {
        Event = @event;
    }

    // ReSharper disable once Unity.IncorrectMonoBehaviourInstantiation
    public void Execute() {
        var context = new OverrideContext { Priority = Event.Priority };
        var definition = Assets.GetBuildingDef(Event.PrefabId);
        var tool = new T {
            def = definition,
            conduitMgr = definition.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>().GetNetworkManager(),
            selectedElements = Event.Materials,
            path = Event.Path
        };

        GameContextManager.Override(context, () => tool.BuildPath());
    }

}
