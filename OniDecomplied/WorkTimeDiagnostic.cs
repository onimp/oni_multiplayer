// Decompiled with JetBrains decompiler
// Type: WorkTimeDiagnostic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

public class WorkTimeDiagnostic : ColonyDiagnostic
{
  public ChoreGroup choreGroup;

  public WorkTimeDiagnostic(int worldID, ChoreGroup choreGroup)
    : base(worldID, (string) UI.COLONY_DIAGNOSTICS.WORKTIMEDIAGNOSTIC.ALL_NAME)
  {
    this.choreGroup = choreGroup;
    this.tracker = (Tracker) TrackerTool.Instance.GetWorkTimeTracker(worldID, choreGroup);
    this.trackerSampleCountSeconds = 100f;
    this.name = choreGroup.Name;
    this.id = "WorkTimeDiagnostic_" + choreGroup.Id;
    this.colors[ColonyDiagnostic.DiagnosticResult.Opinion.Good] = Constants.NEUTRAL_COLOR;
  }

  public override ColonyDiagnostic.DiagnosticResult Evaluate() => new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS)
  {
    opinion = (double) this.tracker.GetAverageValue(this.trackerSampleCountSeconds) > 0.0 ? ColonyDiagnostic.DiagnosticResult.Opinion.Good : ColonyDiagnostic.DiagnosticResult.Opinion.Normal,
    Message = string.Format((string) UI.COLONY_DIAGNOSTICS.ALLWORKTIMEDIAGNOSTIC.NORMAL, (object) this.tracker.FormatValueString(this.tracker.GetAverageValue(this.trackerSampleCountSeconds)))
  };
}
