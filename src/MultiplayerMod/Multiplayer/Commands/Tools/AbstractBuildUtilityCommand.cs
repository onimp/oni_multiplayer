using System;
using MultiplayerMod.Game.Context;
using MultiplayerMod.Game.UI.Tools.Context;
using MultiplayerMod.Game.UI.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class AbstractBuildUtilityCommand<T> : MultiplayerCommand where T : BaseUtilityBuildTool, new() {

    protected UtilityBuildEventArgs Arguments;

    public AbstractBuildUtilityCommand(UtilityBuildEventArgs arguments) {
        Arguments = arguments;
    }

    // ReSharper disable once Unity.IncorrectMonoBehaviourInstantiation
    public override void Execute(MultiplayerCommandContext context) {
        var definition = Assets.GetBuildingDef(Arguments.PrefabId);
        var tool = new T {
            def = definition,
            conduitMgr = definition.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>().GetNetworkManager(),
            selectedElements = Arguments.Materials,
            path = Arguments.Path
        };

        GameContext.Override(new PrioritySettingsContext(Arguments.Priority), () => tool.BuildPath());
    }

}
