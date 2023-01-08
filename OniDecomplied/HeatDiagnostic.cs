// Decompiled with JetBrains decompiler
// Type: HeatDiagnostic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;

public class HeatDiagnostic : ColonyDiagnostic
{
  public HeatDiagnostic(int worldID)
    : base(worldID, (string) UI.COLONY_DIAGNOSTICS.HEATDIAGNOSTIC.ALL_NAME)
  {
    this.tracker = (Tracker) TrackerTool.Instance.GetWorldTracker<BatteryTracker>(worldID);
    this.trackerSampleCountSeconds = 4f;
    this.AddCriterion("CheckHeat", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.HEATDIAGNOSTIC.CRITERIA.CHECKHEAT, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckHeat)));
  }

  private ColonyDiagnostic.DiagnosticResult CheckHeat() => new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS)
  {
    opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal,
    Message = (string) UI.COLONY_DIAGNOSTICS.BATTERYDIAGNOSTIC.NORMAL
  };
}
