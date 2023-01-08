// Decompiled with JetBrains decompiler
// Type: StressDiagnostic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StressDiagnostic : ColonyDiagnostic
{
  public StressDiagnostic(int worldID)
    : base(worldID, (string) UI.COLONY_DIAGNOSTICS.STRESSDIAGNOSTIC.ALL_NAME)
  {
    this.tracker = (Tracker) TrackerTool.Instance.GetWorldTracker<StressTracker>(worldID);
    this.icon = "mod_stress";
    this.AddCriterion("CheckStressed", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.STRESSDIAGNOSTIC.CRITERIA.CHECKSTRESSED, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckStressed)));
  }

  private ColonyDiagnostic.DiagnosticResult CheckStressed()
  {
    List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(this.worldID);
    ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
    if (worldItems.Count == 0)
    {
      diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
      string str = (string) (TrackerTool.Instance.IsRocketInterior(this.worldID) ? UI.COLONY_DIAGNOSTICS.ROCKET : UI.CLUSTERMAP.PLANETOID_KEYWORD);
      diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.NO_MINIONS;
    }
    else
    {
      diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
      diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.STRESSDIAGNOSTIC.NORMAL;
      if ((double) this.tracker.GetAverageValue(this.trackerSampleCountSeconds) >= (double) TUNING.STRESS.ACTING_OUT_RESET)
      {
        diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
        diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.STRESSDIAGNOSTIC.HIGH_STRESS;
        MinionIdentity minionIdentity = worldItems.Find((Predicate<MinionIdentity>) (match => (double) ((Component) match).gameObject.GetAmounts().GetValue(Db.Get().Amounts.Stress.Id) >= (double) TUNING.STRESS.ACTING_OUT_RESET));
        if (Object.op_Inequality((Object) minionIdentity, (Object) null))
          diagnosticResult.clickThroughTarget = new Tuple<Vector3, GameObject>(((Component) minionIdentity).gameObject.transform.position, ((Component) minionIdentity).gameObject);
      }
    }
    return diagnosticResult;
  }
}
