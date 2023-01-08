// Decompiled with JetBrains decompiler
// Type: RocketsInOrbitDiagnostic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class RocketsInOrbitDiagnostic : ColonyDiagnostic
{
  private int numRocketsInOrbit;

  public RocketsInOrbitDiagnostic(int worldID)
    : base(worldID, (string) UI.COLONY_DIAGNOSTICS.ROCKETINORBITDIAGNOSTIC.ALL_NAME)
  {
    this.icon = "icon_errand_rocketry";
    this.AddCriterion("RocketsOrbiting", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.ROCKETINORBITDIAGNOSTIC.CRITERIA.CHECKORBIT, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckOrbit)));
  }

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public ColonyDiagnostic.DiagnosticResult CheckOrbit()
  {
    AxialI myWorldLocation1 = ClusterManager.Instance.GetWorld(this.worldID).GetMyWorldLocation();
    ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.NO_MINIONS);
    diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
    this.numRocketsInOrbit = 0;
    Clustercraft clustercraft = (Clustercraft) null;
    bool flag = false;
    foreach (Clustercraft component in Components.Clustercrafts.Items)
    {
      AxialI myWorldLocation2 = component.GetMyWorldLocation();
      AxialI destination = component.Destination;
      if (AxialI.op_Inequality(myWorldLocation2, myWorldLocation1) && ClusterGrid.Instance.IsInRange(myWorldLocation2, myWorldLocation1) && ClusterGrid.Instance.IsInRange(myWorldLocation1, destination))
      {
        ++this.numRocketsInOrbit;
        clustercraft = component;
        flag = flag || !component.CanLandAtAsteroid(myWorldLocation1, false);
      }
    }
    diagnosticResult.Message = this.numRocketsInOrbit != 1 || !Object.op_Inequality((Object) clustercraft, (Object) null) ? (this.numRocketsInOrbit <= 0 ? (string) UI.COLONY_DIAGNOSTICS.ROCKETINORBITDIAGNOSTIC.NORMAL_NO_ROCKETS : string.Format((string) (flag ? UI.COLONY_DIAGNOSTICS.ROCKETINORBITDIAGNOSTIC.WARNING_ROCKETS_STRANDED : UI.COLONY_DIAGNOSTICS.ROCKETINORBITDIAGNOSTIC.NORMAL_IN_ORBIT), (object) this.numRocketsInOrbit)) : string.Format((string) (flag ? UI.COLONY_DIAGNOSTICS.ROCKETINORBITDIAGNOSTIC.WARNING_ONE_ROCKETS_STRANDED : UI.COLONY_DIAGNOSTICS.ROCKETINORBITDIAGNOSTIC.NORMAL_ONE_IN_ORBIT), (object) clustercraft.Name);
    if (flag)
      diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Warning;
    else if (this.numRocketsInOrbit > 0)
      diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Suggestion;
    return diagnosticResult;
  }
}
