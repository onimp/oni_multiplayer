using System;

namespace MultiplayerMod.Game.Debug;

[Serializable]
public class ModifyParameters {
    public bool AffectCells { get; set; }
    public bool AffectBuildings { get; set; }
    public SimHashes? Element { get; set; }
    public byte? DiseaseType { get; set; }
    public int? DiseaseCount { get; set; }
    public float? Temperature { get; set; }
    public float? Mass { get; set; }
    public bool PreventFowReveal { get; set; }
    public bool AllowFowReveal { get; set; }
}
