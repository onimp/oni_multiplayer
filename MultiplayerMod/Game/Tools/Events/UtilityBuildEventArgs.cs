using System;
using System.Collections.Generic;

namespace MultiplayerMod.Game.Tools.Events;

[Serializable]
public class UtilityBuildEventArgs {
    public string PrefabId { get; set; }
    public Tag[] Materials { get; set; }
    public List<BaseUtilityBuildTool.PathNode> Path { get; set; }
    public PrioritySetting Priority { get; set; }
}
