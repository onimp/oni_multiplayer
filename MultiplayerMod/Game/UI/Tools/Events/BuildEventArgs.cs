using System;

namespace MultiplayerMod.Game.UI.Tools.Events;

[Serializable]
public class BuildEventArgs : EventArgs {
    public int Cell { get; }
    public string PrefabId { get; }
    public bool InstantBuild { get; }
    public bool Upgrade { get; }
    public Orientation Orientation { get; }
    public Tag[] Materials { get; }
    public string FacadeId { get; }
    public PrioritySetting Priority { get; }

    public BuildEventArgs(
        int cell,
        string prefabId,
        bool instantBuild,
        bool upgrade,
        Orientation orientation,
        Tag[] materials,
        string facadeId,
        PrioritySetting priority
    ) {
        Cell = cell;
        PrefabId = prefabId;
        InstantBuild = instantBuild;
        Upgrade = upgrade;
        Orientation = orientation;
        Materials = materials;
        FacadeId = facadeId;
        Priority = priority;
    }
}
