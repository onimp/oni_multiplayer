using System;
using MultiplayerMod.Game.UI.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class BuildWire : AbstractBuildUtilityCommand<UtilityBuildTool> {
    public BuildWire(UtilityBuildEventArgs arguments) : base(arguments) { }
}
