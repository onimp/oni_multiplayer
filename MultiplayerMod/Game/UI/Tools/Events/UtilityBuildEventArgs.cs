using System;
using System.Collections.Generic;

namespace MultiplayerMod.Game.UI.Tools.Events;

[Serializable]
public class UtilityBuildEventArgs {
    public string PrefabId { get; }
    public Tag[] Materials { get; }
    public List<BaseUtilityBuildTool.PathNode> Path { get; }
    public PrioritySetting Priority { get; }

    public UtilityBuildEventArgs(
        string prefabId,
        Tag[] materials,
        List<BaseUtilityBuildTool.PathNode> path,
        PrioritySetting priority
    ) {
        PrefabId = prefabId;
        Materials = materials;
        Path = path;
        Priority = priority;
    }
}
