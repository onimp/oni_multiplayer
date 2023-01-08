// Decompiled with JetBrains decompiler
// Type: RocketFuelDiagnostic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class RocketFuelDiagnostic : ColonyDiagnostic
{
  public RocketFuelDiagnostic(int worldID)
    : base(worldID, (string) UI.COLONY_DIAGNOSTICS.ROCKETFUELDIAGNOSTIC.ALL_NAME)
  {
    this.tracker = (Tracker) TrackerTool.Instance.GetWorldTracker<RocketFuelTracker>(worldID);
    this.icon = "rocket_fuel";
  }

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override ColonyDiagnostic.DiagnosticResult Evaluate()
  {
    Clustercraft component = ((Component) ClusterManager.Instance.GetWorld(this.worldID)).gameObject.GetComponent<Clustercraft>();
    ColonyDiagnostic.DiagnosticResult result = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.NO_MINIONS);
    if (ColonyDiagnosticUtility.IgnoreRocketsWithNoCrewRequested(this.worldID, out result))
      return result;
    result.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
    result.Message = (string) UI.COLONY_DIAGNOSTICS.ROCKETFUELDIAGNOSTIC.NORMAL;
    if ((double) component.ModuleInterface.FuelRemaining == 0.0)
    {
      result.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
      result.Message = (string) UI.COLONY_DIAGNOSTICS.ROCKETFUELDIAGNOSTIC.WARNING;
    }
    return result;
  }
}
