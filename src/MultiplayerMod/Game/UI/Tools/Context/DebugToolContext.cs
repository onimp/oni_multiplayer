using System;
using MultiplayerMod.Game.Context;

namespace MultiplayerMod.Game.UI.Tools.Context;

[Serializable]
public class DebugToolContext : IGameContext {

    public bool AffectCells { get; set; }
    public bool AffectBuildings { get; set; }
    public SimHashes? Element { get; set; }
    public byte? DiseaseType { get; set; }
    public int? DiseaseCount { get; set; }
    public float? Temperature { get; set; }
    public float? Mass { get; set; }
    public bool PreventFowReveal { get; set; }
    public bool AllowFowReveal { get; set; }

    public void Apply() {
        GameStaticContext.Override();
        var instance = GameStaticContext.Current.DebugPaintElementScreen;
        if (Element != null) {
            instance.paintElement.isOn = true;
            instance.element = Element.Value;
        }
        if (Temperature != null) {
            instance.paintTemperature.isOn = true;
            instance.temperature = Temperature.Value;
        }
        if (Mass != null) {
            instance.paintMass.isOn = true;
            instance.mass = Mass.Value;
        }
        if (DiseaseCount != null) {
            instance.paintDiseaseCount.isOn = true;
            instance.diseaseCount = DiseaseCount.Value;
        }
        if (DiseaseType != null) {
            instance.paintDisease.isOn = true;
            instance.diseaseIdx = DiseaseType.Value;
        }

        instance.affectBuildings.isOn = AffectBuildings;
        instance.affectCells.isOn = AffectCells;
        instance.set_prevent_fow_reveal = PreventFowReveal;
        instance.set_allow_fow_reveal = AllowFowReveal;
    }

    public void Restore() {
        GameStaticContext.Restore();
    }

}
