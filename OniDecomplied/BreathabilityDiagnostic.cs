// Decompiled with JetBrains decompiler
// Type: BreathabilityDiagnostic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BreathabilityDiagnostic : ColonyDiagnostic
{
  public BreathabilityDiagnostic(int worldID)
    : base(worldID, (string) UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.ALL_NAME)
  {
    this.tracker = (Tracker) TrackerTool.Instance.GetWorldTracker<BreathabilityTracker>(worldID);
    this.trackerSampleCountSeconds = 50f;
    this.icon = "overlay_oxygen";
    this.AddCriterion("CheckSuffocation", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.CRITERIA.CHECKSUFFOCATION, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckSuffocation)));
    this.AddCriterion("CheckLowBreathability", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.CRITERIA.CHECKLOWBREATHABILITY, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckLowBreathability)));
  }

  private ColonyDiagnostic.DiagnosticResult CheckSuffocation()
  {
    List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(this.worldID);
    if (worldItems.Count != 0)
    {
      foreach (MinionIdentity cmp in worldItems)
      {
        ((Component) cmp).GetComponent<OxygenBreather>().GetGasProvider();
        SuffocationMonitor.Instance smi = ((Component) cmp).GetSMI<SuffocationMonitor.Instance>();
        if (smi != null && smi.IsInsideState((StateMachine.BaseState) smi.sm.nooxygen.suffocating))
          return new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.DuplicantThreatening, (string) UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.SUFFOCATING, new Tuple<Vector3, GameObject>(smi.transform.position, smi.gameObject));
      }
    }
    return new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
  }

  private ColonyDiagnostic.DiagnosticResult CheckLowBreathability() => Components.LiveMinionIdentities.GetWorldItems(this.worldID).Count != 0 && (double) this.tracker.GetAverageValue(this.trackerSampleCountSeconds) < 60.0 ? new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Concern, (string) UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.POOR) : new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);

  public override ColonyDiagnostic.DiagnosticResult Evaluate()
  {
    ColonyDiagnostic.DiagnosticResult result = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.NO_MINIONS);
    return ColonyDiagnosticUtility.IgnoreRocketsWithNoCrewRequested(this.worldID, out result) ? result : base.Evaluate();
  }
}
