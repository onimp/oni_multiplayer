// Decompiled with JetBrains decompiler
// Type: IdleDiagnostic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class IdleDiagnostic : ColonyDiagnostic
{
  public IdleDiagnostic(int worldID)
    : base(worldID, (string) UI.COLONY_DIAGNOSTICS.IDLEDIAGNOSTIC.ALL_NAME)
  {
    this.tracker = (Tracker) TrackerTool.Instance.GetWorldTracker<IdleTracker>(worldID);
    this.icon = "icon_errand_operate";
    this.AddCriterion("CheckIdle", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.IDLEDIAGNOSTIC.CRITERIA.CHECKIDLE, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckIdle)));
  }

  private ColonyDiagnostic.DiagnosticResult CheckIdle()
  {
    List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(this.worldID);
    ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
    if (worldItems.Count == 0)
    {
      diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
      diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.NO_MINIONS;
    }
    else
    {
      diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
      diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.IDLEDIAGNOSTIC.NORMAL;
      if ((double) this.tracker.GetMinValue(30f) > 0.0 && (double) this.tracker.GetCurrentValue() > 0.0)
      {
        diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
        diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.IDLEDIAGNOSTIC.IDLE;
        MinionIdentity minionIdentity = Components.LiveMinionIdentities.GetWorldItems(this.worldID).Find((Predicate<MinionIdentity>) (match => ((Component) match).HasTag(GameTags.Idle)));
        if (Object.op_Inequality((Object) minionIdentity, (Object) null))
          diagnosticResult.clickThroughTarget = new Tuple<Vector3, GameObject>(minionIdentity.transform.position, ((Component) minionIdentity).gameObject);
      }
    }
    return diagnosticResult;
  }
}
