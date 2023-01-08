// Decompiled with JetBrains decompiler
// Type: AllChoresDiagnostic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

public class AllChoresDiagnostic : ColonyDiagnostic
{
  public AllChoresDiagnostic(int worldID)
    : base(worldID, (string) UI.COLONY_DIAGNOSTICS.ALLCHORESDIAGNOSTIC.ALL_NAME)
  {
    this.tracker = (Tracker) TrackerTool.Instance.GetWorldTracker<AllChoresCountTracker>(worldID);
    this.colors[ColonyDiagnostic.DiagnosticResult.Opinion.Good] = Constants.NEUTRAL_COLOR;
    this.icon = "icon_errand_operate";
  }

  public override ColonyDiagnostic.DiagnosticResult Evaluate() => new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS)
  {
    opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal,
    Message = string.Format((string) UI.COLONY_DIAGNOSTICS.ALLCHORESDIAGNOSTIC.NORMAL, (object) this.tracker.FormatValueString(this.tracker.GetCurrentValue()))
  };
}
