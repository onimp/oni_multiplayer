using System;
using MultiplayerMod.Game.Context;

namespace MultiplayerMod.Game.Tools.Context;

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
        if (Element != null) {
            GameStaticContext.Current.DebugPaintElementScreen.paintElement.isOn = true;
            GameStaticContext.Current.DebugPaintElementScreen.element = Element.Value;
        }
        if (Temperature != null) {
            GameStaticContext.Current.DebugPaintElementScreen.paintTemperature.isOn = true;
            GameStaticContext.Current.DebugPaintElementScreen.temperature = Temperature.Value;
        }
        if (Mass != null) {
            GameStaticContext.Current.DebugPaintElementScreen.paintMass.isOn = true;
            GameStaticContext.Current.DebugPaintElementScreen.mass = Mass.Value;
        }
        if (DiseaseCount != null) {
            GameStaticContext.Current.DebugPaintElementScreen.paintDiseaseCount.isOn = true;
            GameStaticContext.Current.DebugPaintElementScreen.diseaseCount = DiseaseCount.Value;
        }
        if (DiseaseType != null) {
            GameStaticContext.Current.DebugPaintElementScreen.paintDisease.isOn = true;
            GameStaticContext.Current.DebugPaintElementScreen.diseaseIdx = DiseaseType.Value;
        }

        GameStaticContext.Current.DebugPaintElementScreen.affectBuildings.isOn = AffectBuildings;
        GameStaticContext.Current.DebugPaintElementScreen.affectCells.isOn = AffectCells;
        GameStaticContext.Current.DebugPaintElementScreen.set_prevent_fow_reveal = PreventFowReveal;
        GameStaticContext.Current.DebugPaintElementScreen.set_allow_fow_reveal = AllowFowReveal;
    }

    public void Restore() {
        GameStaticContext.Restore();
    }

}
