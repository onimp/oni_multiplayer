// Decompiled with JetBrains decompiler
// Type: RocketOxidizerDiagnostic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class RocketOxidizerDiagnostic : ColonyDiagnostic
{
  public RocketOxidizerDiagnostic(int worldID)
    : base(worldID, (string) UI.COLONY_DIAGNOSTICS.ROCKETOXIDIZERDIAGNOSTIC.ALL_NAME)
  {
    this.tracker = (Tracker) TrackerTool.Instance.GetWorldTracker<RocketOxidizerTracker>(worldID);
    this.icon = "rocket_oxidizer";
  }

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override ColonyDiagnostic.DiagnosticResult Evaluate()
  {
    Clustercraft component = ((Component) ClusterManager.Instance.GetWorld(this.worldID)).gameObject.GetComponent<Clustercraft>();
    ColonyDiagnostic.DiagnosticResult result = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.NO_MINIONS);
    if (ColonyDiagnosticUtility.IgnoreRocketsWithNoCrewRequested(this.worldID, out result))
      return result;
    result.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
    result.Message = (string) UI.COLONY_DIAGNOSTICS.ROCKETOXIDIZERDIAGNOSTIC.NORMAL;
    RocketEngineCluster engine = component.ModuleInterface.GetEngine();
    if ((double) component.ModuleInterface.OxidizerPowerRemaining == 0.0 && Object.op_Inequality((Object) engine, (Object) null) && engine.requireOxidizer)
    {
      result.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
      result.Message = (string) UI.COLONY_DIAGNOSTICS.ROCKETOXIDIZERDIAGNOSTIC.WARNING;
    }
    return result;
  }
}
