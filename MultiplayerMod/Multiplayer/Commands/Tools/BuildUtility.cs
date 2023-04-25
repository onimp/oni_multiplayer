using System;
using MultiplayerMod.Game.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class BuildUtility : AbstractBuildUtilityCommand<UtilityBuildTool> {
    public BuildUtility(UtilityBuildEventArgs arguments) : base(arguments) { }
}
