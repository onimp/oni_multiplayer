using System;
using MultiplayerMod.Game.Events.Tools;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class BuildWire : AbstractBuildUtilityCommand<UtilityBuildTool> {
    public BuildWire(UtilityBuildEventArgs @event) : base(@event) { }
}
