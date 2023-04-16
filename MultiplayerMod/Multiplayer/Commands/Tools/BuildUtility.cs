using System;
using MultiplayerMod.Game.Events.Tools;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class BuildUtility : AbstractBuildUtilityCommand<UtilityBuildTool> {
    public BuildUtility(UtilityBuildEventArgs @event) : base(@event) { }
}
