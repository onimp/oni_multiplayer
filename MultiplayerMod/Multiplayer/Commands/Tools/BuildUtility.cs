using System;
using MultiplayerMod.Game.UI.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class BuildUtility : AbstractBuildUtilityCommand<UtilityBuildTool> {
    public BuildUtility(UtilityBuildEventArgs arguments) : base(arguments) { }
}
