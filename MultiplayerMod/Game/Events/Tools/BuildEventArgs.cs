using System;

namespace MultiplayerMod.Game.Events.Tools;

[Serializable]
public class BuildEventArgs {
    public int Cell { get; set; }
    public string PrefabId { get; set; }
    public bool InstantBuild { get; set; }
    public bool Upgrade { get; set; }
    public Orientation Orientation { get; set; }
    public Tag[] Materials { get; set; }
    public string FacadeId { get; set; }
    public PrioritySetting Priority { get; set; }
}
