using System;

namespace MultiplayerMod.Game.Tools.Events;

[Serializable]
public class BuildEventArgs : EventArgs {
    public int Cell { get; set; }
    public string PrefabId { get; set; }
    public bool InstantBuild { get; set; }
    public bool Upgrade { get; set; }
    public Orientation Orientation { get; set; }
    public Tag[] Materials { get; set; }
    public string FacadeId { get; set; }
    public PrioritySetting Priority { get; set; }
}
