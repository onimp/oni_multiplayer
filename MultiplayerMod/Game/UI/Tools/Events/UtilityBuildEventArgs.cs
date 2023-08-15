using System;
using System.Collections.Generic;

namespace MultiplayerMod.Game.UI.Tools.Events;

[Serializable]
public record UtilityBuildEventArgs(
    string PrefabId,
    Tag[] Materials,
    List<BaseUtilityBuildTool.PathNode> Path,
    PrioritySetting Priority
);
